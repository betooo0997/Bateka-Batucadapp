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
    GameObject home_title, other_title;

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
        }
        else if(title == "Hide")
        {
            home_title.SetActive(false);
            other_title.SetActive(false);
        }
        else
        {
            home_title.SetActive(false);
            other_title.SetActive(true);

            Title.text = title;

            back_button.gameObject.SetActive(action != null);

            if (action != null)
            {
                back_button.onClick.RemoveAllListeners();
                back_button.onClick.AddListener(action);
            }
        }
    }
}
