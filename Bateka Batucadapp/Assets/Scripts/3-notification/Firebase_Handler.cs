using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;
using System;

public class Firebase_Handler : MonoBehaviour
{
    public enum Operation
    {
        add,
        remove
    }

    public static Firebase_Handler Singleton;
    public static FirebaseApp App;
    public static bool Init_Success;
    public static string Own_Registration_Token;

    public GoogleAnalyticsV4 GoogleAnalytics;

    static string proj_id = "421329234128";
    static string auth_key = "AAAAYhkt6NA:APA91bHqzOWKt0HpxDP4W-pOJ9h2c-I0MGRYc6TTniNI29PmNGkkvg_V2Cw-8iD4FTnrWOkfavBlUv-OaQJTP9WQNNlQDT5IDXIpuxROjEZUNVpsKM9q1PWviWs6uGkgU1XwwltnlaTq";
    static char s = '"';

    static Dictionary<string, string> default_header;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        #if UNITY_EDITOR
            Own_Registration_Token = "cvJ8X3-zdv4:APA91bFOM5pVWiUx_Wr4QyNXw1FRpH-W__Wh5RuXSEDwrHzPfxd5vDRic4H3KTDrkUtwHPWfLNQxhWnLNpaMDVPK62Sc_ucfbUxSEstRpZDx5QSS0dwk7oclJkK2Tn4QbOg8AKFAjbKA";
        #endif

        default_header = new Dictionary<string, string>()
        {
                {"Content-Type", "application/json" },
                {"Authorization", "key=" + auth_key },
                {"project_id" , proj_id }
        };

        GoogleAnalytics.StartSession();
    }

    public static void Set_User_Property(string key, string value)
    {
        if (Init_Success)
        {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty(key, value);
            Debug.Log("Firebase: user property set successfully. Key: " + key + ", Value: " + value);
        }
        else
            Debug.LogError("Firebase error: user property '" + key + "' could not be set due to failure on Firebase initialization.");
    }

    // EVENTS

    public static void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Firebase: registration token is " + token.Token);
        Own_Registration_Token = token.Token;
    }

    public static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        string message = "Firebase: received new message from: " + e.Message.From + "\n" +
                         "Data (" + e.Message.Data.Count.ToString() + "):\n";

        foreach (KeyValuePair<string, string> entry in e.Message.Data)
            message += "Key: " + entry.Key + ", Value: " + entry.Value + "\n";

        Debug.Log(message);
    }



    // ______________________________________
    //
    // CREATE NOTIFICATION KEY.
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



    // ______________________________________
    //
    // GET NOTIFICATION KEY.
    // ______________________________________
    //


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
    // ADD / REMOVE REGISTRATION TOKEN FROM DEVICE GROUP.
    // ______________________________________
    //


    public static void Modify_Registration_Token(Operation operation, string notification_key = "", string notification_key_name = "", string registration_token = "", Action<object[]> concluding_method = null)
    {
        if (notification_key == "")
            notification_key = "APA91bFo2yaFWEkDkHaknxnUPXBR7YuzctajvF_XIvWbo6ts_DzHPHMU1IZ44aNC7P0VejrtTC-fQqRVfctfDy5oMwIkspC6fAh6dNknDWRNFKyvLp7JeIg";

        if (notification_key_name == "")
            notification_key_name = "user_id" + User.User_Info.Id.ToString();

        if (registration_token == "")
            registration_token = Own_Registration_Token;

        string content =
        "{" +
            s + "operation" + s + ":" + s + operation.ToString() + s + "," +
            s + "notification_key_name" + s + ":" + s + notification_key_name + s + "," +
            s + "notification_key" + s + ":" + s + notification_key + s + "," +
            s + "registration_ids" + s + ":[" + s + registration_token + s + "]" +
        "}";

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification",
            Http_Client.Request_Type.POST,
            default_header,
            Modify_Registration_Token_Response,
            concluding_method,
            content
            );
    }

    static void Modify_Registration_Token_Response(object[] data, Action<object[]> concluding_method)
    {
        Response_Status status = (Response_Status)Enum.Parse(typeof(Response_Status), data[0].ToString());

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

    public static void Send_Notification(string notification_key = "", string title = "Title test", string body = "Body Test", Action<object[]> concluding_method = null, string priority = "high", Dictionary<string, string> data_pairs = null)
    {
        #if UNITY_EDITOR
            notification_key = "APA91bHjTjDMZ5RxFhutJXbakqhjXqwWYl3x3SHoGdRTXRo_FcW-Ascx3pOa6b934gT_kFr5LXfRWIQ-ea2FABuV2uzNiIC7InN21yUFd3S4jqukpIUomaE";
        #endif

        string content = 
            "{" +
                 s + "to" + s + ":" + s + notification_key + s + "," +
                 s + "data" + s + 
                 ":{";

        if (data_pairs != null)
            foreach (KeyValuePair<string, string> data in data_pairs)
                content += s + data.Key + s + ":" + s + data.Value + s + ",";

        if (content[content.Length - 1] == ',')
            content = content.Substring(0, content.Length - 1);

        content += 
                "}" +
                s + "priority" + s + ": " + s + priority + s + "," +
                s + "notification" + s + ": " + 
                "{" +
                    s + "title" + s + ": " + s + title + s + "," +
                    s + "body" + s + ": " + s + body + s + 
                "}" +
            "}";

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/send",
            Http_Client.Request_Type.POST,
            default_header,
            On_Response,
            concluding_method,
            content
            );
    }

    static void On_Response(object[] data, Action<object[]> concluding_method)
    {
    }
}
