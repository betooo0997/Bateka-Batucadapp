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

    [SerializeField]
    GameObject event_overview_prefab;

    [SerializeField]
    LayoutElement horizontal_scrollview;

    [SerializeField]
    RectTransform month_element;

    Calendar_Month[] months;

    int current_month_element;

    DateTime month_to_show;

    Vector2 previous_mouse_pos;


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

    // ______________________________________
    //
    // 3. SHOW OVERVIEW.
    // ______________________________________
    //


    public void Show_Overview(Calendar_Day day)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
            if(button.name.Contains("overview"))
                Destroy(button.gameObject);

        foreach (Calendar_Event calendar_event in day.Calendar_events)
        {
            Calendar_Overview overview = Instantiate(event_overview_prefab, months[1].transform).GetComponent<Calendar_Overview>();
            overview.SetValues(calendar_event);
        }
    }
}
