using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Votable : Data_struct
{
    public Privacy Vote_Privacy;
    public Votable_Type Votable_Type;
    public List<List<User.User_Information>> Vote_Voters;
    public List<string> Vote_Types;
    public DateTime Answering_Deadline;
    public List<Comment> Comments;

    public Votable()
    {
        Vote_Voters = new List<List<User.User_Information>>();
        Vote_Types = new List<string>();
        Answering_Deadline = new DateTime();
        Comments = new List<Comment>();
    }
}

public enum Votable_Type
{
    Yes_No = Menu.Menu_item.Poll_details_yes_no,
    Other = Menu.Menu_item.Poll_details_other
}

[System.Serializable]
public struct Comment
{
    public string Author;
    public string Content;
}