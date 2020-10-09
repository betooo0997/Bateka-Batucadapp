using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using System;
using System.Collections;

public class Firebase_Handler : MonoBehaviour
{
    public class FCM_Params
    {
        public uint User_Id = 0;
        public string Notification_Key = "";
        public string Notification_Key_Name = "";
        public string Registration_Token = "";
        public string Priority = "10";
        public string Title = "";
        public string Body = "";

        public Dictionary<string, string> Data_Pairs = null;
        public Action<object[]> Concluding_Method = null;
    }

    public enum Operation
    {
        add,
        remove
    }

    public static Firebase_Handler Singleton;
    public static FirebaseApp App;
    public static bool Init_Success;

    public static string Own_Registration_Token = "";
    public static string Own_Notification_Key = "";

    public GoogleAnalyticsV4 GoogleAnalytics;

    static Dictionary<string, string> default_header;
    static string proj_id;
    const string server_key = "AAAAYhkt6NA:APA91bHqzOWKt0HpxDP4W-pOJ9h2c-I0MGRYc6TTniNI29PmNGkkvg_V2Cw-8iD4FTnrWOkfavBlUv-OaQJTP9WQNNlQDT5IDXIpuxROjEZUNVpsKM9q1PWviWs6uGkgU1XwwltnlaTq";
    const char s = '"';



    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFECYCLE AND INITIALIZATION.
    // ______________________________________
    //


    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        GoogleAnalytics.StartSession();
    }

    void Update()
    {
        if(Init_Success)
        {
            StartCoroutine(Initialize_());
            enabled = false;
        }
    }

    IEnumerator Initialize_()
    {
        App = FirebaseApp.DefaultInstance;
        FirebaseApp.LogLevel = LogLevel.Info;
        proj_id = App.Options.MessageSenderId;

        default_header = new Dictionary<string, string>()
        {
                {"Content-Type", "application/json" },
                {"Authorization", "key=" + server_key },
                {"project_id" , proj_id }
        };

        while(!User.Initialized)
            yield return null;

        /*Firebase_Handler.Singleton.GoogleAnalytics.LogScreen("Login_S");
        Firebase_Handler.Singleton.GoogleAnalytics.LogEvent("Category_Example", "Event_Action", "Event_Label", 1);
        Firebase_Handler.Singleton.GoogleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("login").SetEventAction("login"));
        Firebase_Handler.Singleton.GoogleAnalytics.DispatchHits();*/

        Set_User_Property("user_id", User.User_Info.Id.ToString());

        #if !UNITY_EDITOR
            Firebase_Handler.Modify_Registration_Token(Firebase_Handler.Operation.add);
        #endif

        Debug.Log("Firebase initialized successfully. Data:" +
            "\nProjectId: " + App.Options.ProjectId +
            "\nMessageSenderId: " + App.Options.MessageSenderId +
            "\nAppId: " + App.Options.AppId +
            "\nApiKey: " + App.Options.ApiKey);

        yield return null;
    }



    // ______________________________________
    //
    // 2. EVENT HANDLING.
    // ______________________________________
    //


    public static void On_Token_Received(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Firebase: registration token is " + token.Token);
        Own_Registration_Token = token.Token;
    }

    public static void On_Message_Received(object sender, MessageReceivedEventArgs e)
    {
        string message = "Firebase: received new message from: " + e.Message.From + "\n" +
                         "Data (" + e.Message.Data.Count.ToString() + "):\n";

        foreach (KeyValuePair<string, string> entry in e.Message.Data)
            message += "Key: " + entry.Key + ", Value: " + entry.Value + "\n";

        Debug.Log(message);
        Notification_UI_Pop.Show_Firebase_Message(e.Message.Data);
    }



    // ______________________________________
    //
    // 3. USER PROPERTY SETTING.
    // ______________________________________
    //


    public static void Set_User_Property(string key, string value)
    {
        Firebase.Analytics.FirebaseAnalytics.SetUserProperty(key, value);
        Debug.Log("Firebase: user property set successfully. Key: " + key + ", Value: " + value);
    }



    // ______________________________________
    //
    // 4. NOTIFICATION KEY HANDLING.
    // ______________________________________
    //


    public static void Create_Notification_Key(string notification_key_name = "", string registration_token = "", Action<object[]> concluding_method = null)
    {
        if (notification_key_name == "" || registration_token == "")
        {
            notification_key_name = "user_id" + User.User_Info.Id.ToString();
            registration_token = Own_Registration_Token;
        }

        string content =
            "{" +
                 s + "operation" + s + ":" + s + "create" + s + "," +
                 s + "notification_key_name" + s + ":" + s + notification_key_name + s + "," +
                 s + "registration_ids" + s + ":[" + s + registration_token + s + "]" +
             "}";

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification",
            Http_Client.Request_Type.POST,
            default_header,
            Get_Notification_Key_Response,
            concluding_method,
            content
            );
    }

    public static void Get_Notification_Key(string notification_key_name = "", Action<object[]> concluding_method = null)
    {
        if (notification_key_name == "")
            notification_key_name = "user_id" + User.User_Info.Id.ToString();

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification?notification_key_name=" + notification_key_name,
            Http_Client.Request_Type.GET,
            default_header,
            Get_Notification_Key_Response,
            concluding_method
            );
    }

    static void Get_Notification_Key_Response(object[] data, Action<object[]> concluding_method)
    {
        Response_Status status = Http_Client.Parse_Status(data[0]);

        if (status < Response_Status.Network_Error)
        {
            SimpleJSON.JSONNode raw = SimpleJSON.JSON.Parse((string)data[2]);

            if (status == Response_Status.Ok)
            {
                if (raw["notification_key"] != null)
                {
                    Debug.Log("Notification key is " + raw["notification_key"].Value);
                    concluding_method?.Invoke(new object[] { status, raw["notification_key"].Value });
                }
                else
                {
                    Debug.LogError("Correct HTTP response but notification key is null!");
                }
            }
            else
                concluding_method?.Invoke(new object[] { status, raw["error"].Value });
        }
    }



    // ______________________________________
    //
    // 5. REGISTRATION TOKEN HANDLING.
    // ______________________________________
    //


    public static void Modify_Registration_Token(Operation operation, FCM_Params param = null)
    {
        if (param == null)
            param = new FCM_Params();

        if (param.Notification_Key_Name == "")
            param.Notification_Key_Name = "user_id" + User.User_Info.Id.ToString();

        if (param.Registration_Token == "")
            param.Registration_Token = Own_Registration_Token;

        if (param.Notification_Key.Length <= 1)
        {
            if (Own_Notification_Key.Length > 1)
                param.Notification_Key = Own_Notification_Key;
            else
            {
                Get_Notification_Key("", (object[] data_get) =>
                {
                    switch (Http_Client.Parse_Status(data_get[0]))
                    {
                        case Response_Status.Ok:
                            Modify(data_get);
                            break;

                        case Response_Status.HTTP_Error:
                            if ((string)data_get[1] == "notification_key not found")
                            {
                                Create_Notification_Key("", "", (object[] data_create) =>
                                {
                                    if (Http_Client.Parse_Status(data_create[0]) == Response_Status.Ok)
                                        Modify(data_create);
                                    else
                                        Debug.LogError("Could not get notifiaction_key!");

                                    param.Concluding_Method?.Invoke(data_create);
                                });
                            }
                            else
                                param.Concluding_Method?.Invoke(data_get);
                            break;

                        default:
                            Debug.LogError("Could not get the notification_key of notification_key_name " + param.Notification_Key_Name);
                            param.Concluding_Method?.Invoke(data_get);
                            break;
                    }

                    void Modify(object[] data_param)
                    {
                        Modify_Registration_Token(operation, new FCM_Params()
                        {
                            Notification_Key = (string)data_param[1],
                            Concluding_Method = (object[] data_modify) =>
                            {
                                if (Http_Client.Parse_Status(data_modify[0]) == Response_Status.Ok)
                                    Debug.Log("Registration_token has been " + operation + "ed successfully from notification_key_name " + param.Notification_Key_Name);
#if !UNITY_EDITOR
                                else
                                    Debug.LogError("Could not " + operation.ToString() + " the registration_token " + param.Registration_Token + " from notification_key_name " +
                                        param.Notification_Key_Name + " and notification_key " + param.Notification_Key);
#endif
                                param.Concluding_Method?.Invoke(data_modify);
                            }
                        });
                    }
                });
                return;
            }
        }

        string content =
        "{" +
            s + "operation" + s + ":" + s + operation.ToString() + s + "," +
            s + "notification_key_name" + s + ":" + s + param.Notification_Key_Name + s + "," +
            s + "notification_key" + s + ":" + s + param.Notification_Key + s + "," +
            s + "registration_ids" + s + ":[" + s + param.Registration_Token + s + "]" +
        "}";

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification",
            Http_Client.Request_Type.POST,
            default_header,
            Get_Notification_Key_Response,
            param.Concluding_Method,
            content
            );
    }



    // ______________________________________
    //
    // 6. NOTIFICATION SENDING.
    // ______________________________________
    //


    public static void Send_Notification(FCM_Params param)
    {
        if (param.Notification_Key == "")
        {
            Get_Notification_Key("user_id" + param.User_Id.ToString(), (object[] data_get) =>
            {
                Response_Status status_get = Http_Client.Parse_Status(data_get[0]);

                if (status_get == Response_Status.Ok)
                {
                    param.Notification_Key = (string)data_get[1];
                    Send_Notification(param);
                }
                else
                {
                    Debug.LogWarning("Could not send notification due to error on getting the notification_key!");
                    param.Concluding_Method?.Invoke(new object[] { Response_Status.HTTP_Error, "notification_key not found", "-1" });
                }
            });
            return;
        }

        string content = 
            "{" +
                 s + "to" + s + ":" + s + param.Notification_Key + s + "," +
                 s + "data" + s + 
                 ":{";

        if (param.Data_Pairs != null)
            foreach (KeyValuePair<string, string> data in param.Data_Pairs)
                content += s + data.Key + s + ":" + s + data.Value + s + ",";

        if (content[content.Length - 1] == ',')
            content = content.Substring(0, content.Length - 1);

        content += 
                "}" +
                s + "notification" + s + ": " + 
                "{" +
                    s + "title" + s + ": " + s + param.Title + s + "," +
                    s + "body" + s + ": " + s + param.Body + s + "," +
                    s + "priority" + s + ": " + param.Priority +
                "}" +
            "}";

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/send",
            Http_Client.Request_Type.POST,
            default_header,
            Send_Notification_Response,
            param.Concluding_Method,
            content
            );
    }

    static void Send_Notification_Response(object[] data, Action<object[]> concluding_method)
    {
        Response_Status status = Http_Client.Parse_Status(data[0]);

        if (status < Response_Status.Network_Error)
        {
            SimpleJSON.JSONNode raw = SimpleJSON.JSON.Parse((string)data[2]);

            if (status == Response_Status.Ok)
            {
                if (raw["failure"].Value == "0")
                {
                    Debug.Log("Success on sending notification.");
                    concluding_method?.Invoke(new object[] { status, raw["success"].Value, "0"});
                }
                else
                {
                    Debug.LogWarning("Correct HTTP response but sending failures exists! (" + raw["success"].Value + " success, " + raw["failure"].Value + " failures)");
                    concluding_method?.Invoke(new object[] { status, raw["success"].Value, raw["failure"].Value });
                }
            }
            else
                concluding_method?.Invoke(new object[] { status, raw["error"].Value, "-1" });
        }
    }



    // ______________________________________
    //
    // 7. TOPIC SUBSCRIPTION HANDLING.
    // ______________________________________
    //

    // TODO. https://developers.google.com/instance-id/reference/server
}
