using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Polls : MonoBehaviour
{
    /// <summary>
    /// A Comment of somebody on a Poll.
    /// </summary>
    [System.Serializable]
    public struct Comment
    {
        public string Author;
        public string Content;
    }

    /// <summary>
    /// Poll struct with all its details.
    /// </summary>
    [System.Serializable]
    public class Poll
    {
        public uint Id;
        public string Title;
        public string Subtitle;
        public string Description;
        public string Creation_time;
        public string Author;
        public string Privacy;
        public List<User.User_Information> Favour;
        public List<User.User_Information> Abstention;
        public List<User.User_Information> Opposed;
        public List<User.User_Information> Blank;
        public List<Comment> Comments;
    }

    /// <summary>
    /// The different types a vote can have.
    /// </summary>
    public enum VoteType
    {
        Favour,
        Abstention,
        Opposed,
        Blank
    }

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
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_Poll_Response(string response)
    {
        Parse_Poll_Data(response, true);
    }

    static void AddVoter(ref List<User.User_Information> vote_list, string data)
    {
        string[] user_ids = Utils.Split(data, ',');

        foreach (string user_id in user_ids)
        {
            User.User_Information voter = User.Get_User(uint.Parse(user_id));
            if (voter.Id != 0) vote_list.Add(voter);
        }
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
            // Separate information and comment section from database.
            string[] data_split = Utils.Split(poll, "\\COMMENTS");
            Poll newPoll = new Poll
            {
                Comments = new List<Comment>(),
                Favour = new List<User.User_Information>(),
                Abstention = new List<User.User_Information>(),
                Opposed = new List<User.User_Information>(),
                Blank = new List<User.User_Information>()
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

                    case "favour":
                        AddVoter(ref newPoll.Favour, tokens[1]);
                        break;

                    case "abstention":
                        AddVoter(ref newPoll.Abstention, tokens[1]);
                        break;

                    case "opposed":
                        AddVoter(ref newPoll.Opposed, tokens[1]);
                        break;

                    case "blank":
                        AddVoter(ref newPoll.Blank, tokens[1]);
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

            Poll_List.Add(newPoll);
        }
    }

    /// <summary>
    /// Updates user's choice of vote locally on the device and remotely on the server.
    /// </summary>
    public void Vote(uint poll_id, VoteType vote_type)
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd", "vote_pollid", "vote_type" };
        string[] field_values = { "set_poll_vote", User.User_Info.Username, User.User_Info.Psswd, poll_id.ToString(), vote_type.ToString().ToLower() };
        Http_Client.Send_Post(field_names, field_values, Handle_Vote_Response);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_Vote_Response(string response)
    {
        Message.ShowMessage("Voto registrado correctamente");
    }
}