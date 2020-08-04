#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title_Handler : MonoBehaviour
{
    public static Title_Handler Singleton;

    [SerializeField]
    Button back_button;

    [SerializeField]
    GameObject home_title, other_title, notification_button, edit_button;

    [SerializeField]
    Text Title;

    void Awake()
    {
        Singleton = this;
    }

    public void Set_Title(string title = "", UnityEngine.Events.UnityAction action = null)
    {
        if(title == "")
        {
            home_title.SetActive(true);
            other_title.SetActive(false);
            return;
        }

        if(title == "Hide")
        {
            home_title.SetActive(false);
            other_title.SetActive(false);
            return;
        }

        other_title.SetActive(true);
        home_title.SetActive(false);
        notification_button.SetActive(title == "Usuarios" && User.User_Info.Role == User.User_Role.admin);
        edit_button.SetActive(User.User_Info.Role == User.User_Role.admin && (
            Menu.Active_Item == Menu.Menu_item.Events_details ||
            Menu.Active_Item == Menu.Menu_item.Poll_details_multi ||
            Menu.Active_Item == Menu.Menu_item.Poll_details_single ||
            Menu.Active_Item == Menu.Menu_item.News_details));

        Title.text = title;

        back_button.gameObject.SetActive(action != null);

        if (action != null)
        {
            back_button.onClick.RemoveAllListeners();
            back_button.onClick.AddListener(action);
        }
    }
}
