using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events : Database_Handler
{
    static List<Calendar_Events_section> sections;

    private new void Awake()
    {
        sections = new List<Calendar_Events_section>();
    }



    // ______________________________________
    //
    // 1. LOAD DATA.
    // ______________________________________
    //


    public static Calendar_Event Parse_Single_Data(string news_entry_data)
    {
        Calendar_Event calendar_event = new Calendar_Event();

        foreach (string element in Utils.Split(news_entry_data, '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch (tokens[0])
            {
                case "id":
                    calendar_event.Id = uint.Parse(tokens[1]);
                    break;

                case "title":
                    calendar_event.Title = tokens[1];
                    break;

                case "details":
                    calendar_event.Details = tokens[1];
                    break;

                case "location":
                    calendar_event.Location = tokens[1];
                    break;

                case "date":
                    calendar_event.Date = Utils.Get_DateTime(tokens[1]);
                    break;

                case "confirm_deadline":
                    calendar_event.Confirm_Deadline = Utils.Get_DateTime(tokens[1]);
                    break;
            }
        }

        return calendar_event;
    }
}
