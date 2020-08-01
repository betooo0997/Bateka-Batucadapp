using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll_Updater : MonoBehaviour
{
    public static Scroll_Updater Singleton;

    public static Dictionary<Type, bool> Initialized;

    bool updating;

    Image image;

    RectTransform scroll_view_content;
    RectTransform load_icon;

    public static int User_Loaded = 1;

    static bool download_all;

    public static void Initialize()
    {
        Initialized = new Dictionary<Type, bool>();
        Initialized.Add(typeof(News), false);
        Initialized.Add(typeof(Polls), false);
        Initialized.Add(typeof(Calendar_Events), false);
        Initialized.Add(typeof(Docs), false);
        download_all = false;
    }

    private void Awake()
    {
        Singleton = this;
        image = GetComponent<Image>();
        load_icon = transform.GetChild(0).gameObject.GetComponent<RectTransform>();
        scroll_view_content = transform.parent.GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!download_all || Database_Handler.Singleton != null && !Initialized[Database_Handler.Singleton.GetType()] || (!updating && scroll_view_content.localPosition.y < -50 && Input.GetMouseButtonUp(0)))
        {
            if (!download_all)
            {
                Database_Handler.Load_Data_Server(Handler_Type.events);
                Database_Handler.Load_Data_Server(Handler_Type.news);
                Database_Handler.Load_Data_Server(Handler_Type.polls);
                Database_Handler.Load_Data_Server(Handler_Type.docs);
                download_all = true;
            }
            else
            {
                if (Menu.Active_Item == Menu.Menu_item.Home)
                    Database_Handler.Load_Data_Server(Handler_Type.events);
                else if (Menu.Active_Item != Menu.Menu_item.Users)
                    Database_Handler.Load_Data_Server((Handler_Type)Menu.Active_Item);

                if (Initialized[typeof(Calendar_Events)])
                    User.Update_Data("", "", false, true);
            }

            updating = true;
            load_icon.gameObject.SetActive(true);
            load_icon.sizeDelta = new Vector3(load_icon.sizeDelta.x, 60);
            Utils.Update_UI = true;
        }
        else if (!updating && User_Loaded >= 1 && Singleton.load_icon.sizeDelta.y > 0)
        {
            Singleton.load_icon.sizeDelta = new Vector3(Singleton.load_icon.sizeDelta.x, Singleton.load_icon.sizeDelta.y - Time.deltaTime * 250);

            if (Singleton.load_icon.sizeDelta.y < 0)
            {
                Singleton.load_icon.sizeDelta = new Vector3(Singleton.load_icon.sizeDelta.x, 0);
                Singleton.load_icon.gameObject.SetActive(false);

                if (Menu.Active_Item == Menu.Menu_item.Home)
                    Utils.Update_UI = true;
            }
        }
    }

    public static void Disable()
    {
        if (Singleton != null)
            Singleton.updating = false;
    }
}
