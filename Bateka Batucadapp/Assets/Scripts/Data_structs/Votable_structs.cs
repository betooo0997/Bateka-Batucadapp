using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Votable : Data_struct
{
    public Votable()
    {
        Vote_Voters = new List<List<User.User_Information>>();
        Vote_Types = new List<string>();
        Answering_Deadline = new DateTime();
        Comments = new List<Comment>();
    }

    public Privacy Vote_Privacy;
    public List<List<User.User_Information>> Vote_Voters;
    public List<string> Vote_Types;
    public DateTime Answering_Deadline;
    public List<Comment> Comments;
}