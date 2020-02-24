using System.Collections;
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
        Load_Data_Cache();
    }

    static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("user_database"))
        {
            Loading_Screen.Set_Active(true);
            Login.Parse_Login_Data(PlayerPrefs.GetString("user_database"));
            Loading_Screen.Set_Active(true);
            Database_Handler.Load_Data_Cache(Handler_Type.news);
            Database_Handler.Load_Data_Cache(Handler_Type.events);
            Database_Handler.Load_Data_Cache(Handler_Type.polls);
        }
    }
}
