#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Calendar_Handler : MonoBehaviour
{
    public static Calendar_Handler Singleton;

    Calendar_Month[] months;

    DateTime month_to_show;



    // ______________________________________
    //
    // 1. INITIALIZE.
    // ______________________________________
    //


    public void Initialize()
    {
        months[0].Initialize(month_to_show.AddMonths(-1));
        months[1].Initialize(month_to_show);
        months[2].Initialize(month_to_show.AddMonths(1));

        foreach (Scrollable scrollable in FindObjectsOfType<Scrollable>())
            scrollable.Initialize();
    }



    // ______________________________________
    //
    // 2. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    void Awake()
    {
        Singleton = this;        
        months = GetComponentsInChildren<Calendar_Month>();
    }

    void Start()
    {
        // Get first of month.
        month_to_show = DateTime.Now.AddDays(-DateTime.Now.Day + 1); 
        Initialize();

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(Screen.width * 3 / Utils.Canvas_Scale, rect.sizeDelta.y);
    }

    public void OnFinish(int change_value)
    {
        month_to_show = month_to_show.AddMonths(change_value);

        months[0].Initialize(month_to_show.AddMonths(-1));
        months[1].Initialize(month_to_show);
        months[2].Initialize(month_to_show.AddMonths(1));
    }
}
