using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public static Initializer Singleton;

    void Awake()
    {
        Singleton = this;
        Database_Handler.Initialize_Dictionaries();
        Scroll_Updater.Initialize();
    }

    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => 
        {
            var dependencyStatus = task.Result;
            Firebase_Handler.Init_Success = (dependencyStatus == Firebase.DependencyStatus.Available);

            if (!Firebase_Handler.Init_Success)
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

        TouchScreenKeyboard.hideInput = true;
        Load_Data_Cache();
    }

    static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("user_database") && PlayerPrefs.HasKey("version") && PlayerPrefs.GetString("version") == Application.version.ToString() && Database_Handler.Is_Clean(PlayerPrefs.GetString("user_database")))
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
}
