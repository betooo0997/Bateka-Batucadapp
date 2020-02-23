using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Polls : Database_Handler
{
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

        // Separate information and comment section from database.
        string[] data_split = Utils.Split(poll_data, "\\COMMENTS");

        Poll newPoll = new Poll
        {
            Comments = new List<Comment>(),
            Vote_Voters = new List<List<User.User_Information>>(),
            Vote_Types = new List<string>()
        };

        // Parse information section.
        foreach (string element in Utils.Split(data_split[0], '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch (tokens[0])
            {
                case "id":
                    newPoll.Id = uint.Parse(tokens[1]);
                    break;

                case "title":
                    newPoll.Title = tokens[1];
                    break;

                case "subtitle":
                    newPoll.Subtitle = tokens[1];
                    break;

                case "description":
                    newPoll.Details = tokens[1];
                    break;

                case "creation_time":
                    newPoll.Creation_Time = Utils.Get_DateTime(tokens[1]);
                    break;

                case "author":
                    newPoll.Author = tokens[1];
                    break;

                case "privacy":
                    newPoll.Vote_Privacy = Utils.Parse_Privacy(tokens[1]);
                    break;

                case "expiration_time":
                    newPoll.Answering_Deadline = Utils.Get_DateTime(tokens[1]);
                    break;

                case "options":
                    string[] options = Utils.Split(tokens[1], "+");

                    foreach (string option in options)
                    {
                        string[] node = Utils.Split(option, "@");
                        newPoll.Vote_Types.Add(node[0]);
                        if(node.Length == 2)
                            newPoll.Vote_Voters.Add(Utils.Get_Voters(node[1]));
                    }
                    break;
            }
        }

        // Parse comment section.
        foreach (string commentNode in Utils.Split(data_split[1], '#'))
        {
            string[] comment_elements = Utils.Split(commentNode, '~');
            Comment newComment = new Comment();

            foreach (string comment_element in comment_elements)
            {
                string[] tokens = Utils.Split(comment_element, '^');

                if (tokens.Length != 2) continue;
                switch (tokens[0])
                {
                    case "author":
                        newComment.Author = tokens[1];
                        break;

                    case "content":
                        newComment.Content = tokens[1];
                        break;
                }
            }

            newPoll.Comments.Add(newComment);
        }

        // Set Poll Type and Poll Status.
        newPoll.Type = Poll_Type.Other;
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
            newPoll.Type = Poll_Type.Yes_No;

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
}