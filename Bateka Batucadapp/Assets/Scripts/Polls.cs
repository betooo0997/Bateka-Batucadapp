using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Polls : MonoBehaviour
{
    [System.Serializable]
    public struct Comment
    {
        public string author;
        public string data;
    }

    [System.Serializable]
    public struct Poll
    {
        public uint Id;
        public string Title;
        public string Subtitle;
        public string Description;
        public string Creation_time;
        public string Author;
        public string Privacy;
        public uint Favour;
        public uint Abstention;
        public uint Opposed;
        public uint Blank;
        public List<Comment> Comments;
    }

    public static List<Poll> Poll_List;

    void Awake()
    {
        if (Poll_List == null) Poll_List = new List<Poll>();
    }

    void Start()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_polls", User.User_Info.Username, User.User_Info.Psswd };
        Http_Client.Send_Post(field_names, field_values, Handle_Poll_Response);
    }

    void Handle_Poll_Response(string response)
    {
        string data = Utils.Split(response, '|')[1];
        string[] data_split = Utils.Split(data, "\\COMMENTS");
        Poll newPoll = new Poll();

        foreach (string element in Utils.Split(data_split[0], '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch(tokens[0])
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
                    newPoll.Favour = uint.Parse(tokens[1]);
                    break;

                case "abstention":
                    newPoll.Abstention = uint.Parse(tokens[1]);
                    break;

                case "opposed":
                    newPoll.Opposed = uint.Parse(tokens[1]);
                    break;

                case "blank":
                    newPoll.Blank = uint.Parse(tokens[1]);
                    break;
            }
        }

        Poll_List.Add(newPoll);
    }
}