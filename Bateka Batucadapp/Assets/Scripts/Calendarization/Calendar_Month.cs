using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Month : MonoBehaviour
{
    public DateTime Date;

    List<Calendar_Day> days;

    [SerializeField]
    Text month_text;

    private void Awake()
    {
        days = Utils.GetComponentsInChildren_Order<Calendar_Day>(transform);
    }

    public void Initialize(DateTime month)
    {
        this.Date = month;
        month_text.text = month.ToString("MMMM") + " " + month.Year;

        // Get monday before first of the month.
        DateTime first_day = month.AddDays(-Get_Distance_To_Monday(month.DayOfWeek));
        DateTime temp = first_day;

        foreach (Calendar_Day day in days)
        {
            day.Set_date(temp, this);
            List<Data_struct> list = new List<Data_struct>();

            // Set Calendar_Day values.
            foreach (Calendar_Event data in Database_Handler.Data_List_Get(typeof(Calendar_Events)))
                if (data.Date_Event.Year == temp.Year && data.Date_Event.Month == temp.Month && data.Date_Event.Day == temp.Day)
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
}
