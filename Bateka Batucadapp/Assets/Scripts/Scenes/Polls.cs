using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Polls : MonoBehaviour
{
    /// <summary>
    /// List of all downloaded Polls.
    /// </summary>
    public static List<Poll> Poll_List;
    public List<Poll> Ppoll_List;

    public static Poll Selected_Poll;

    [SerializeField]
    GameObject poll_UI_prefab;

    [SerializeField]
    Transform poll_UI_parent;



    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    void Start()
    {
        if (!PlayerPrefs.HasKey("poll_database"))
        {
            string[] field_names = { "REQUEST_TYPE" };
            string[] field_values = { "get_polls" };
            Http_Client.Send_Post(field_names, field_values, Handle_Poll_Response);
        }
        Ppoll_List = Poll_List;
    }

    private void Update()
    {
        if (Poll_List != null && Poll_List.Count > 0)
        {
            Spawn_Poll_UIs();
            enabled = false;
        }
    }



    // ______________________________________
    //
    // 2. PARSE POLL DATABASE(S).
    // ______________________________________
    //


    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_Poll_Response(string response)
    {
        Parse_Poll_Data(response, true);
        Ppoll_List = Poll_List;
    }

    /// <summary>
    /// Parses poll databases from a server response.
    /// </summary>
    /// <param name="response">Poll database to be parsed.</param>
    ///  <param name="save">If response should be saved to PlayerPrefs. It is assumed that if it's false, the response originates from the PlayerPrefs.</param>
    public static void Parse_Poll_Data(string response, bool save = false)
    {
        Poll_List = new List<Poll>();

        if (save)
            DataSaver.Save_Database("poll_database", response);

        else if (PlayerPrefs.HasKey("poll_database_timestamp"))
            Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("user_database_timestamp"));

        // Separate poll database from initial server information. (E.g. "VERIFIED.|*poll databases*|")
        string data = Utils.Split(response, '|')[1];

        // Separate each database to parse it individually. (E.g. "*database_1*_PDBEND_*database_2*")
        foreach (string poll in Utils.Split(data, "_PDBEND_"))
            Poll_List.Add(Parse_Single_Poll_Data(poll));
    }

    /// <summary>
    /// Parses a single poll database.
    /// </summary>
    public static Poll Parse_Single_Poll_Data(string poll_data)
    {
        if (poll_data.Contains("|"))
            poll_data = Utils.Split(poll_data, '|')[1];

        poll_data = poll_data.Replace("_PDBEND_", "");

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
                    newPoll.Description = tokens[1];
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
        Poll_Type type = Poll_Type.Other;
        string status = "";

        for (int y = 0; y < newPoll.Vote_Voters.Count; y++)
        {
            for (int x = 0; x < newPoll.Vote_Voters[y].Count; x++)
            {
                if (newPoll.Vote_Voters[y][x].Id == User.User_Info.Id)
                {
                    status = newPoll.Vote_Types[y];
                    newPoll.Selected_Option_Idx = y;
                    break;
                }
            }
        }

        if (newPoll.Vote_Voters.Count == 2 && newPoll.Vote_Types[0] == "affirmation" && newPoll.Vote_Types[1] == "rejection")
            type = Poll_Type.Yes_No;

        if (newPoll.Vote_Voters.Count < 2)
            Debug.LogError("No options detected.");

        newPoll.Status = status;
        newPoll.Type = type;

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
    // 3. UPDATE (OR CREATE) POLLS BY ADMIN.
    // ______________________________________
    //


    public void Update_Poll(Poll poll)
    {
        Debug.Log("Updating Poll");
        string[] field_names = { "REQUEST_TYPE", "poll_id", "poll_data" };
        string[] field_values = { "set_poll", poll.Id.ToString(), poll.Convert_To_String() };
        Http_Client.Send_Post(field_names, field_values, Handle_Poll_Update_Response);
    }

    void Handle_Poll_Update_Response(string response)
    {
    }



    // ______________________________________
    //
    // 4. HANDLE UI.
    // ______________________________________
    //


    /// <summary>
    /// Spawns all Poll_UI elements.
    /// </summary>
    void Spawn_Poll_UIs()
    {
        foreach (Poll poll in Poll_List)
        {
            GameObject new_Poll = Instantiate(poll_UI_prefab, poll_UI_parent);
            new_Poll.name = "Poll_" + poll.Id;
            new_Poll.GetComponent<Poll_UI>().Set_Values(poll);
        }
    }
}