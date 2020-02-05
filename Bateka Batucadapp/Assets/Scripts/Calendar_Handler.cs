using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Handler : MonoBehaviour
{
    DateTime first_day;

    [SerializeField]
    GameObject week_prefab;

    [SerializeField]
    GameObject day_prefab;

    [SerializeField]
    Text month;

    public static Calendar_Handler Singleton;

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

        for (int x = 0; x < 6; x++)
        {
            GameObject week = Instantiate(week_prefab, transform);
            week.name = "Week_instance";

            for (int y = 0; y < 7; y++)
            {
                GameObject day = Instantiate(day_prefab, week.transform);
                day.name = "Day_instance";
                Calendar_Day calendar_day = day.GetComponent<Calendar_Day>();
                calendar_day.Set_date(temp);

                // Set Calendar_Day values.
                foreach (Calendar_Event data in Database_Handler.Data_List_Get(typeof(Calendar_Events)))
                {
                    if (data.Date.Year == temp.Year && data.Date.Month == temp.Month && data.Date.Day == temp.Day)
                    {
                        calendar_day.Set_event(data);
                        break;
                    }
                }

                temp = temp.AddDays(1);
            }

            Canvas.ForceUpdateCanvases();
            week.GetComponent<HorizontalLayoutGroup>().SetLayoutHorizontal();
        }

        Canvas.ForceUpdateCanvases();
        GetComponent<VerticalLayoutGroup>().SetLayoutVertical();
    }

    int Get_Distance_To_Monday(DayOfWeek week_day)
    {
        if (week_day == DayOfWeek.Sunday)
            return 6;

        return (int)week_day - 1;
    }
}
