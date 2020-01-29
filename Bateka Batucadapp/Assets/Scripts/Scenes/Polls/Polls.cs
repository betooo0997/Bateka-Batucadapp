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
                    newPoll.Creation_time = tokens[1];
                    break;

                case "author":
                    newPoll.Author = tokens[1];
                    break;

                case "privacy":
                    newPoll.Privacy = tokens[1];
                    break;

                case "options":
                    string[] options = Utils.Split(tokens[1], "+");

                    foreach (string option in options)
                    {
                        string[] node = Utils.Split(option, "@");
                        newPoll.Vote_Types.Add(node[0]);
                        if(node.Length == 2)
                            newPoll.Vote_Voters.Add(Get_Voters(node[1]));
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

    /// <summary>
    /// Get list of users from a data string.
    /// </summary>
    static List<User.User_Information> Get_Voters(string data)
    {
        string[] user_ids = Utils.Split(data, ',');
        List<User.User_Information> vote_list = new List<User.User_Information>();

        foreach (string user_id in user_ids)
        {
            User.User_Information voter = User.Get_User(uint.Parse(user_id));
            if (voter.Id != 0)
                vote_list.Add(voter);
        }

        return vote_list;
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
}