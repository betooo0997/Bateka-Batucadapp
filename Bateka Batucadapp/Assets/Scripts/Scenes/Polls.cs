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

    void Start()
    {
        if (!PlayerPrefs.HasKey("poll_database"))
        {
            string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
            string[] field_values = { "get_polls", User.User_Info.Username, User.User_Info.Psswd };
            Http_Client.Send_Post(field_names, field_values, Handle_Poll_Response);
        }
        Ppoll_List = Poll_List;
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_Poll_Response(string response)
    {
        Parse_Poll_Data(response, true);
        Ppoll_List = Poll_List;
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
        {
            Poll_List.Add(Parse_Single_Poll_Data(poll));
        }
    }

    static Poll Parse_Single_Poll_Data(string poll_data)
    {
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

        return newPoll;
    }

    /// <summary>
    /// Updates user's choice of vote locally on the device and remotely on the server.
    /// </summary>
    public void Vote(uint poll_id, string vote_type)
    {
        Debug.Log("Voting");
        string[] field_names = { "REQUEST_TYPE", "username", "psswd", "vote_poll_id", "vote_type" };
        string[] field_values = { "set_poll_vote", User.User_Info.Username, User.User_Info.Psswd, poll_id.ToString(), vote_type };
        Http_Client.Send_Post(field_names, field_values, Handle_Vote_Response);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_Vote_Response(string response)
    {
        Poll updated_poll = Parse_Single_Poll_Data(response);

        for (int x = 0; x < Poll_List.Count; x++)
        {
            if (Poll_List[x].Id == updated_poll.Id)
            {
                Poll_List[x] = updated_poll;
                break;
            }
        }

        Ppoll_List = Poll_List;
        Message.ShowMessage("Base de datos actualizada con éxito.");
    }

    public void Update_Poll(Poll poll)
    {
        Debug.Log("Updating Poll");
        string[] field_names = { "REQUEST_TYPE", "username", "psswd", "poll_id", "poll_data" };
        string[] field_values = { "set_poll", User.User_Info.Username, User.User_Info.Psswd, poll.Id.ToString(), poll.Convert_To_String() };
        Http_Client.Send_Post(field_names, field_values, Handle_Poll_Update_Response);
    }

    void Handle_Poll_Update_Response(string response)
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Vote(0, "a");

        if (Input.GetKeyDown(KeyCode.B))
            Vote(0, "b");

        if (Input.GetKeyDown(KeyCode.C))
            Vote(0, "c");

        if (Input.GetKeyDown(KeyCode.D))
            Vote(0, "dddddddddd");

        if (Input.GetKeyDown(KeyCode.P))
            Update_Poll(Poll_List[0]);
    }
}