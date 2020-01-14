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
        public string content;
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

    public List<Poll> Poll_List;

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

        foreach (string poll in Utils.Split(data, "_PDBEND_"))
        {
            string[] data_split = Utils.Split(poll, "\\COMMENTS");
            Poll newPoll = new Poll { Comments = new List<Comment>() };

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

            foreach (string commentNode in Utils.Split(data_split[1], '#'))
            {
                string[] comment_elements = Utils.Split(commentNode, '~');
                string author, content = "";
                Comment newComment = new Comment();

                foreach (string comment_element in comment_elements)
                {
                    string[] tokens = Utils.Split(comment_element, '^');

                    if (tokens.Length != 2) continue;
                    switch (tokens[0])
                    {
                        case "author":
                            newComment.author = tokens[1];
                            break;

                        case "content":
                            newComment.content = tokens[1];
                            break;
                    }
                }

                newPoll.Comments.Add(newComment);
            }

            Poll_List.Add(newPoll);
        }
    }
}