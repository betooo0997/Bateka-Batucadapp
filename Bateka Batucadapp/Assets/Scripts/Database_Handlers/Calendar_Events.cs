using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events : Database_Handler
{
    static List<Calendar_Events_section> sections;

    public GameObject Data_Section_UI_Prefab;

    protected override void Awake()
    {
        base.Awake();
        sections = new List<Calendar_Events_section>();
    }

    

    // ______________________________________
    //
    // 1. LOAD DATA.
    // ______________________________________
    //


    public static Calendar_Event Parse_Single_Data(string news_entry_data)
    {
        news_entry_data = Utils.Clear_Response(news_entry_data);
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

                case "meeting_time":
                    calendar_event.Meeting_Time = Utils.Get_DateTime(tokens[1]);
                    break;

                case "meeting_location":
                    calendar_event.Meeting_Location = tokens[1];
                    break;

                case "options":
                    string[] options = Utils.Split(tokens[1], "+");

                    foreach (string option in options)
                    {
                        string[] node = Utils.Split(option, "@");
                        calendar_event.Vote_Types.Add(node[0]);
                        if (node.Length == 2)
                            calendar_event.Vote_Voters.Add(Utils.Get_Voters(node[1]));
                    }
                    break;
            }
        }

        for (int y = 0; y < calendar_event.Vote_Voters.Count; y++)
        {
            for (int x = 0; x < calendar_event.Vote_Voters[y].Count; x++)
            {
                if (calendar_event.Vote_Voters[y][x].Id == User.User_Info.Id)
                {
                    calendar_event.Status = calendar_event.Vote_Types[y];
                    break;
                }
            }
        }

        return calendar_event;
    }

    public static List<Data_struct> Sort_List()
    {
        List<Data_struct> Unsorted_List = Calendar_Events.Data_List_Get(typeof(Calendar_Events));
        DateTime[] date_Times = new DateTime[Unsorted_List.Count];

        for (int x = 0; x < date_Times.Length; x++)
            date_Times[x] = ((Calendar_Event)Unsorted_List[x]).Date;

        List<Data_struct> Sorted_List = Utils.Bubble_Sort_DateTime(Unsorted_List, date_Times);
        return Sorted_List;
    }

    public static void On_Data_Parsed()
    {
        Home.Events_Loaded = true;
    }
}
