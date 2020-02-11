using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Handler : MonoBehaviour
{
    DateTime first_day;

    [SerializeField]
    Text month;

    public static Calendar_Handler Singleton;

    [SerializeField]
    List<Calendar_Day> days;

    [SerializeField]
    GameObject event_overview_prefab;

    [SerializeField]
    RectTransform event_overview_transform;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();

        foreach (Transform element in transforms)
            if (element.name.Contains("instance"))
               Destroy(element.gameObject);

        month.text = DateTime.Now.ToString("MMMM");

        // Get first of month.
        first_day = DateTime.Now.AddDays(-DateTime.Now.Day + 1);
        // Get monday before that.
        first_day = first_day.AddDays(-Get_Distance_To_Monday(first_day.DayOfWeek));

        DateTime temp = first_day;

        foreach (Calendar_Day day in days)
        {
            day.Set_date(temp);
            List<Data_struct> list = new List<Data_struct>();

            // Set Calendar_Day values.
            foreach (Calendar_Event data in Database_Handler.Data_List_Get(typeof(Calendar_Events)))
                if (data.Date.Year == temp.Year && data.Date.Month == temp.Month && data.Date.Day == temp.Day)
                    list.Add(data);

            day.Set_Event(list);
            temp = temp.AddDays(1);
        }
    }

    int Get_Distance_To_Monday(DayOfWeek week_day)
    {
        if (week_day == DayOfWeek.Sunday)
            return 6;

        return (int)week_day - 1;
    }

    public void Show_Overview(Calendar_Day day)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
            if(button.name.Contains("overview"))
                Destroy(button.gameObject);

        foreach (Calendar_Event calendar_event in day.Calendar_events)
        {
            Calendar_Overview overview = Instantiate(event_overview_prefab, transform).GetComponent<Calendar_Overview>();
            overview.SetValues(calendar_event);
        }
    }
}
