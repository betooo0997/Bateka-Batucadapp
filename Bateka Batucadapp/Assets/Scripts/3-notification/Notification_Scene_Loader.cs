using Firebase.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification_Scene_Loader : MonoBehaviour
{
    public static Notification_Scene_Loader Singleton;
    public static uint Data_Id;
    public static Menu.Menu_item Menu_Item;

    void Awake()
    {
        Singleton = this;
    }

    public void Load_Specific_Scene(IDictionary<string, string> data)
    {
        Menu_Item = Menu.Menu_item.Home;

        if (data.ContainsKey("Load_type"))
        {
            Menu_Item = (Menu.Menu_item)Enum.Parse(typeof(Menu.Menu_item), data["Load_type"]);
            Data_Id = uint.Parse(data["Load_id"]);
        }
    }
}
