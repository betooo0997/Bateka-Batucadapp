using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Polls : Database_Handler
{
    [SerializeField]
    Transform parent_not_answered = null;

    [SerializeField]
    Transform parent_answered = null;

    // ______________________________________
    //
    // 1. LOAD DATA.
    // ______________________________________
    //


    /// <summary>
    /// Parses a single poll database.
    /// </summary>
    public static Poll Parse_Single_Data(string poll_data)
    {
        poll_data = Utils.Clear_Response(poll_data);

        Poll newPoll = new Poll
        {
            Comments = new List<Comment>(),
            Vote_Voters = new List<List<User.User_Information>>(),
            Vote_Types = new List<string>()
        };

        string[] data = Utils.Split(poll_data, '#');
        newPoll.Id              = uint.Parse(data[0]);
        newPoll.Title           = data[1];
        newPoll.Details         = data[2];
        newPoll.Creation_Time   = Utils.Get_DateTime(data[3]);
        newPoll.Date_Deadline   = Utils.Get_DateTime(data[4]);
        newPoll.Author          = data[5];
        newPoll.Privacy         = Utils.Parse_Privacy(data[6]);

        foreach (string element in Utils.Split(data[7], "|"))
        {
            newPoll.Vote_Types.Add(element);
            newPoll.Vote_Voters.Add(new List<User.User_Information>());
        }

        foreach (User.User_Information user in User.Users_Info)
            foreach (User.Vote_Data vote_data in user.Polls_Data)
                if (vote_data.id == newPoll.Id)
                    newPoll.Vote_Voters[vote_data.response].Add(user);

        foreach (User.Vote_Data vote_data in User.User_Info.Polls_Data)
            if (vote_data.id == newPoll.Id)
                newPoll.Vote_Voters[vote_data.response].Add(User.User_Info);

        // Set Poll Type and Poll Status.
        newPoll.Votable_Type = Votable_Type.Multiple;
        newPoll.Status = "";

        for (int y = 0; y < newPoll.Vote_Voters.Count; y++)
        {
            for (int x = 0; x < newPoll.Vote_Voters[y].Count; x++)
            {
                if (newPoll.Vote_Voters[y][x].Id == User.User_Info.Id)
                {
                    newPoll.Status = newPoll.Vote_Types[y];
                    newPoll.Selected_Option_Idx = y;
                    break;
                }
            }
        }

        if (newPoll.Vote_Voters.Count == 2 && 
           (newPoll.Vote_Types[0] == "rejection" ^ newPoll.Vote_Types[1] == "rejection") && 
           (newPoll.Vote_Types[0] == "affirmation" ^ newPoll.Vote_Types[1] == "affirmation"))
            newPoll.Votable_Type = Votable_Type.Binary;

        if (newPoll.Vote_Voters.Count < 2)
            Debug.LogError("No options detected.");

        return newPoll;
    }


    // ______________________________________
    //
    // 2. UPDATE (OR CREATE) POLLS BY ADMIN.
    // ______________________________________
    //


    public void Update_Poll(Poll poll)
    {
        Debug.Log("Updating Poll");
        string[] field_names = { "REQUEST_TYPE", "poll_id", "poll_data" };
        string[] field_values = { "set_poll", poll.Id.ToString(), poll.Convert_To_String() };
        Http_Client.Send_Post(field_names, field_values, Handle_Poll_Update_Response);
    }

    void Handle_Poll_Update_Response(string response, Handler_Type type)
    {
    }



    // ______________________________________
    //
    // 3. SORT.
    // ______________________________________
    //


    public static List<Data_struct> Sort_List()
    {
        List<Data_struct> Unsorted_List = Polls.Data_List_Get(typeof(Polls));
        DateTime[] date_Times = new DateTime[Unsorted_List.Count];

        for (int x = 0; x < date_Times.Length; x++)
            date_Times[x] = ((Poll)Unsorted_List[x]).Creation_Time;

        List<Data_struct> Sorted_List = Utils.Bubble_Sort_DateTime(Unsorted_List, date_Times);
        return Sorted_List;
    }

    protected override void Spawn_UI_Elements()
    {
        foreach (Transform transform in Data_UI_Parent.GetComponentsInChildren<Transform>())
            if (transform.name == GetType().ToString() + "_element" || transform.name.Contains(GetType().ToString() + "_section"))
                Destroy(transform.gameObject);

        foreach (Data_struct element in Data_List_Get(GetType()))
        {
            GameObject element_obj = Instantiate(data_UI_prefab, Data_UI_Parent);
            element_obj.name = GetType().ToString() + "_element";
            element_obj.GetComponent<Data_UI>().Set_Data(element);

            if (!Utils.Is_Sooner(((Poll)element).Date_Deadline, DateTime.Now))
            {
                if (((Poll)element).Status == "")
                    element_obj.transform.SetSiblingIndex(parent_not_answered.GetSiblingIndex() + 1);
                else
                    element_obj.transform.SetSiblingIndex(parent_answered.GetSiblingIndex() + 1);
            }
        }

        Utils.Update_UI = true;
        enabled = false;
    }
}