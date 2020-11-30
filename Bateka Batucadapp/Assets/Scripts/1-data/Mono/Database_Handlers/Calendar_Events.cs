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


    public static Calendar_Event Parse_Single_Data(string event_data)
    {
        event_data = Utils.Clear_Response(event_data);
        Calendar_Event calendar_event = new Calendar_Event();

        string[] data = Utils.Split(event_data, '#');

        for (int x = 0; x < data.Length; x++)
            data[x] = Encryption.Decrypt(data[x]);

        calendar_event.Id                   = uint.Parse(data[0]);
        calendar_event.Title                = data[1];
        calendar_event.Details              = data[2];
        calendar_event.Location_Event       = data[3];
        calendar_event.Location_Meeting     = data[4];
        calendar_event.Date_Event           = Utils.Get_DateTime(data[5]);
        calendar_event.Date_Meeting         = Utils.Get_DateTime(data[6]);
        calendar_event.Date_Deadline        = Utils.Get_DateTime(data[7]);
        calendar_event.Transportation       = data[8];
        calendar_event.Cash                 = data[9];
        calendar_event.Food                 = data[10];
        calendar_event.Author_Id            = uint.Parse(data[11]);
        calendar_event.Privacy              = Utils.Parse_Privacy(data[12]);

        calendar_event.Vote_Types.Add("rejection");
        calendar_event.Vote_Types.Add("affirmation");
        calendar_event.Vote_Voters.Add(new List<User.User_Information>());
        calendar_event.Vote_Voters.Add(new List<User.User_Information>());

        foreach (User.User_Information user in User.Users_Info)
            foreach (User.Vote_Data vote_data in user.Events_Data)
                if (vote_data.id == calendar_event.Id)
                    calendar_event.Vote_Voters[vote_data.response].Add(user);

        foreach (User.Vote_Data vote_data in User.User_Info.Events_Data)
            if (vote_data.id == calendar_event.Id)
                calendar_event.Vote_Voters[vote_data.response].Add(User.User_Info);

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
            date_Times[x] = ((Calendar_Event)Unsorted_List[x]).Date_Event;

        List<Data_struct> Sorted_List = Utils.Bubble_Sort_DateTime(Unsorted_List, date_Times, true);
        return Sorted_List;
    }

    protected override void Spawn_UI_Elements()
    {
        base.Spawn_UI_Elements();

        Utils.InvokeNextFrame(() =>
        {
            Calendar_Events_section.Spawn_Sections();
        });
    }
}
