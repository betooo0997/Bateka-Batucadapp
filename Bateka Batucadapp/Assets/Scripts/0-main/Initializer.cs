using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public static Initializer Singleton;
    Firebase.FirebaseApp app;

    void Awake()
    {
        Singleton = this;
        Database_Handler.Initialize_Dictionaries();
        Scroll_Updater.Initialize();
    }

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty("Prueba", "2");
                Firebase.Analytics.FirebaseAnalytics.SetUserProperty("2", "Prueba");
                Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
                Firebase.Messaging.FirebaseMessaging.MessageReceived += OnMessageReceived;
                Debug.Log("Firebase initialized successfully");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }

            Load_Data_Cache();
        });

        TouchScreenKeyboard.hideInput = true;
    }

    static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("user_database") && PlayerPrefs.HasKey("version") && PlayerPrefs.GetString("version") == App_Updater.VERSION.ToString())
        {
            Debug.Log("Loading from cache");
            User.Psswd = PlayerPrefs.GetString("user_psswd");
            User.User_Info.Username = PlayerPrefs.GetString("user_username");
            Login.Singleton.Set_Input_Fields();
            User.Parse_User_Data(PlayerPrefs.GetString("user_database"));
            Database_Handler.Load_Data_Cache(Handler_Type.news);
            Database_Handler.Load_Data_Cache(Handler_Type.events);
            Database_Handler.Load_Data_Cache(Handler_Type.polls);
            Database_Handler.Load_Data_Cache(Handler_Type.docs);

            User.Update_Data("", "", false);
        }
    }

    public void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
    {
        Debug.Log("Received a new message from: " + e.Message.From);
        Debug.Log("Data count: " + e.Message.Data.Count.ToString());

        foreach (KeyValuePair<string, string> entry in e.Message.Data)
        {
            // do something with entry.Value or entry.Key
            Debug.Log("Key: " + entry.Key + ", Value: " + entry.Value);
        }
    }
}
