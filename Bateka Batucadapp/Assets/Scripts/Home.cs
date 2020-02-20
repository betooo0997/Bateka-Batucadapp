using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public static Home Singleton;

    bool events_loaded, news_loaded;

    public static bool Events_Loaded { set { if (Singleton != null) { Singleton.events_loaded = value; Singleton.Check_If_Fully_Loaded(); } } }
    public static bool News_Loaded { set { if (Singleton != null) { Singleton.news_loaded = value; Singleton.Check_If_Fully_Loaded(); } } }

    void Check_If_Fully_Loaded()
    {     
        if(news_loaded && events_loaded)
            Scroll_Updater.Disable();
    }

    private void Awake()
    {
        Singleton = this;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Loading_Screen.Set_Active(false);
    }

    public static void Reset_Load()
    {
        if(Singleton != null)
        {
            Singleton.events_loaded = false;
            Singleton.news_loaded = false;
        }
    }
}
