using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Messaging;

public class Firebase_Handler : MonoBehaviour
{
    public static FirebaseApp App;
    public static bool Init_Success;
    public static string Registration_Token;

    private void Start()
    {
        #if UNITY_EDITOR
            Registration_Token = "cvJ8X3-zdv4:APA91bFOM5pVWiUx_Wr4QyNXw1FRpH-W__Wh5RuXSEDwrHzPfxd5vDRic4H3KTDrkUtwHPWfLNQxhWnLNpaMDVPK62Sc_ucfbUxSEstRpZDx5QSS0dwk7oclJkK2Tn4QbOg8AKFAjbKA";
        #endif
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

    public static void OnTokenReceived(object sender, TokenReceivedEventArgs token)
    {
        Debug.Log("Firebase: registration token is " + token.Token);
        Registration_Token = token.Token;
    }

    public static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        string message = "Firebase: received new message from: " + e.Message.From + "\n" +
                         "Data (" + e.Message.Data.Count.ToString() + "):\n";

        foreach (KeyValuePair<string, string> entry in e.Message.Data)
            message += "Key: " + entry.Key + ", Value: " + entry.Value + "\n";

        Debug.Log(message);
    }

    public static void Check_Device_Group()
    {
        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification?notification_key_name=user_id",
            Http_Client.Request_Type.GET,
            new Dictionary<string, string>()
            {
                {"Content-Type", "application/json" },
                {"Authorization", "key=AAAAYhkt6NA:APA91bHqzOWKt0HpxDP4W-pOJ9h2c-I0MGRYc6TTniNI29PmNGkkvg_V2Cw-8iD4FTnrWOkfavBlUv-OaQJTP9WQNNlQDT5IDXIpuxROjEZUNVpsKM9q1PWviWs6uGkgU1XwwltnlaTq" },
                {"project_id" , "421329234128" }
            },
            On_Response
            );
    }

    public static void Set_Device_Group()
    {
        char s = '"';
        string body = "{\n  " +
                 s + "operation" + s + ": " + s + "create" + s + ",\n  " +
                 s + "notification_key_name" + s + ": " + s + "user_id" + s + ",\n  " +
                 s + "registration_ids" + s + ": [ " + s + Registration_Token + s + " ]\n" +
             "}";

        Debug.LogWarning("body: '" + body + "'");

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/notification",
            Http_Client.Request_Type.POST,
            new Dictionary<string, string>()
            {
                {"Content-Type", "application/json" },
                {"Authorization", "key=AAAAYhkt6NA:APA91bHqzOWKt0HpxDP4W-pOJ9h2c-I0MGRYc6TTniNI29PmNGkkvg_V2Cw-8iD4FTnrWOkfavBlUv-OaQJTP9WQNNlQDT5IDXIpuxROjEZUNVpsKM9q1PWviWs6uGkgU1XwwltnlaTq" },
                {"project_id" , "421329234128" }
            },
            On_Response,
            body
            );
    }

    public static void Send_Notification()
    {
        char s = '"';
        string body = "{\n  " +
                 s + "to" + s + ": " + s + "APA91bHjTjDMZ5RxFhutJXbakqhjXqwWYl3x3SHoGdRTXRo_FcW-Ascx3pOa6b934gT_kFr5LXfRWIQ-ea2FABuV2uzNiIC7InN21yUFd3S4jqukpIUomaE" + s + ",\n  " +
                 s + "data" + s + ": " + "{\n    " +
                 s + "hello" + s + ": " + s + "This is a Firebase Cloud Messaging Device Group Message!" + s + "}"+
                 s + "priority" + s + ": " + s + "high" + s + ",\n  " +
                 s + "notification" + s + ": " + "{\n    " +
                 s + "title" + s + ": " + s + "TitlePasd" + s + "," +
                 s + "body" + s + ": " + s + "Bodyasdasd" + s + "}" +
                 "}";

        Debug.LogWarning("body: '" + body + "'");

        Http_Client.Send_HTTP_Request(
            "https://fcm.googleapis.com/fcm/send",
            Http_Client.Request_Type.POST,
            new Dictionary<string, string>()
            {
                {"Content-Type", "application/json" },
                {"Authorization", "key=AAAAYhkt6NA:APA91bHqzOWKt0HpxDP4W-pOJ9h2c-I0MGRYc6TTniNI29PmNGkkvg_V2Cw-8iD4FTnrWOkfavBlUv-OaQJTP9WQNNlQDT5IDXIpuxROjEZUNVpsKM9q1PWviWs6uGkgU1XwwltnlaTq" },
                {"project_id" , "421329234128" }
            },
            On_Response,
            body
            );
    }

    public static void On_Response(string header, string body)
    {
        Debug.Log("On_Reponse!");
    }
}
