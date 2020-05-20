#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Day : Data_UI
{
    public List<Calendar_Event> Calendar_events;

    [SerializeField]
    Image status;

    static Calendar_Day selected_day;

    static GameObject enabled_borders;

    DateTime date;

    Text day;

    private void Awake()
    {
        day = GetComponentInChildren<Text>();
        GetComponent<Button>().enabled = false;
    }

    public void Set_Event(List<Data_struct> data_struct)
    {
        Calendar_events = data_struct.Cast<Calendar_Event>().ToList();

        if (Calendar_events.Count > 0)
            Update_Color(status);

        else if (date.Day == DateTime.Now.Day && date.Month == DateTime.Now.Month && date.Year == DateTime.Now.Year)
            status.color = new Color(0.9f, 0.9f, 0.9f);

        else
            status.color = new Color(1, 1, 1);
    }

    public void Set_date(DateTime date, Calendar_Month month_instance)
    {
        this.date = date;
        day.text = date.Day.ToString();

        if (date.Month != month_instance.Date.Month)
            day.color = new Color(0.65f, 0.65f, 0.65f);
        else
            day.color = new Color(0, 0, 0);
    }

    protected void Update_Color(Image image)
    {
        switch (Calendar_events[0].Status)
        {
            case "affirmation":
                image.color = color_affirmed(1);
                break;

            case "rejection":
                image.color = color_rejected(1);
                break;

            default:
                image.color = color_not_answered(1);
                break;
        }
    }
}
