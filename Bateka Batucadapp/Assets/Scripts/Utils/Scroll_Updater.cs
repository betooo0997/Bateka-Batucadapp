using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll_Updater : MonoBehaviour
{
    public static Scroll_Updater Singleton;

    static Dictionary<Type, bool> initialized;

    bool updating;

    Image image;

    RectTransform scroll_view_content;
    RectTransform load_icon;

    public static void Initialize()
    {
        initialized = new Dictionary<Type, bool>();
        initialized.Add(typeof(News), false);
        initialized.Add(typeof(Polls), false);
        initialized.Add(typeof(Calendar_Events), false);
        initialized.Add(typeof(Docs), false);
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
        if (!initialized[Database_Handler.Singleton.GetType()] || (!updating && scroll_view_content.localPosition.y < -50 && Input.GetMouseButtonUp(0))) // TODO: Add touch!!
        {
            Database_Handler.Load_Data_Server((Handler_Type)Menu.Active_Item);
            Database_Handler.Load_Data_Server((Handler_Type)Menu.Menu_item.U);

            if (Menu.Active_Item == Menu.Menu_item.Home)
            {
                Home.Reset_Load();
                Database_Handler.Load_Data_Server(Handler_Type.events);
            }

            updating = true;
            load_icon.gameObject.SetActive(true);
            initialized[Database_Handler.Singleton.GetType()] = true;
            Utils.InvokeNextFrame(() => load_icon.sizeDelta = new Vector3(load_icon.sizeDelta.x, 60));
            Utils.InvokeNextFrame(() => FindObjectOfType<ContentSizeFitter>().SetLayoutVertical());
            Utils.InvokeNextFrame(() => Canvas.ForceUpdateCanvases());
            FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
            FindObjectOfType<ContentSizeFitter>().SetLayoutVertical();
            Canvas.ForceUpdateCanvases();
            FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
        }
        else if (!updating && Singleton.load_icon.sizeDelta.y > 0)
        {
            Singleton.load_icon.sizeDelta = new Vector3(Singleton.load_icon.sizeDelta.x, Singleton.load_icon.sizeDelta.y - Time.deltaTime * 250);

            if (Singleton.load_icon.sizeDelta.y < 0)
            {
                Singleton.load_icon.sizeDelta = new Vector3(Singleton.load_icon.sizeDelta.x, 0);
                Singleton.load_icon.sizeDelta = new Vector3(Singleton.load_icon.sizeDelta.x, 0);
                Singleton.load_icon.gameObject.SetActive(false);

                if (Database_Handler.Singleton.GetType() == typeof(News))
                {
                    Canvas.ForceUpdateCanvases();
                    foreach (VerticalLayoutGroup vLayout in FindObjectsOfType<VerticalLayoutGroup>())
                        vLayout.SetLayoutVertical();
                }
            }
        }
    }

    public static void Disable()
    {
        if (Singleton != null)
        {
            Singleton.updating = false;
        }
    }
}
