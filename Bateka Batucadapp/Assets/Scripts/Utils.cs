using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public void Logout()
    {
        User.User_Info = new User.User_Information { Username = "" };
        Polls.Poll_List = new List<Poll>();
        PlayerPrefs.DeleteAll();
        Load_Scene.Load_Scene_ST("Login", false);
    }

    public void Load_Menu_Scene(string scene)
    {
        Menu.Singleton.Load_Scene_Menu_Item(scene);
    }

    public static string[] Split(string to_split, string[] splitter)
    {
        return to_split.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] Split(string to_split, string splitter)
    {
        return to_split.Split(new string[] { splitter }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] Split(string to_split, char splitter)
    {
        return to_split.Split(new char[] { splitter }, System.StringSplitOptions.RemoveEmptyEntries);
    }
}
