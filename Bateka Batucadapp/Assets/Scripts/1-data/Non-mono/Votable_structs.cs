using System;
using System.Collections.Generic;

[System.Serializable]
public abstract class Votable : Data_struct
{
    public Votable_Type Votable_Type;
    List<List<User.User_Information>> vote_voters;
    public List<List<User.User_Information>> Vote_Voters
    {
        get
        {
            for (int x = vote_voters.Count; x < Vote_Types.Count; x++)
                vote_voters.Add(new List<User.User_Information>());

            return vote_voters;
        }
        set { vote_voters = value;  }
    }
    public List<string> Vote_Types;
    public DateTime Date_Deadline;
    public List<Comment> Comments;

    public Votable() : base()
    {
        Vote_Voters     = new List<List<User.User_Information>>();
        Vote_Types      = new List<string>();
        Date_Deadline   = new DateTime();
        Comments        = new List<Comment>();

        editable.Add("Date_Deadline");
    }

    public bool Is_Past_Deadline()
    {
        return Utils.Is_Sooner(Date_Deadline, DateTime.Now);
    }
}

public enum Votable_Type
{
    Multiple_Single_Select = Menu.Menu_item.Poll_details_single,
    Multiple_Multi_Select = Menu.Menu_item.Poll_details_multi,
    Binary
}

[System.Serializable]
public struct Comment
{
    public string Author;
    public string Content;
}