using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public static Initializer Singleton;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        Load_Data_Cache();
    }

    public static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("user_database"))
            Login.Parse_Login_Data(PlayerPrefs.GetString("user_database"));

        Polls.Load_Data_Cache();
        News.Load_Data_Cache();
    }
}
