using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poll : Votable
{
    public string Status;
    public int Selected_Option_Idx = -1;
    public string Subtitle;
    public DateTime Creation_Time;

    public Poll() : base()
    {
        Creation_Time = DateTime.Now;
    }

    string Get_VoteList(List<User.User_Information> list, string nodeName)
    {
        string result = "<" + nodeName + ">";

        foreach (User.User_Information user in list)
            result += user.Id + ",";

        if (result.Contains(","))
            result = result.Substring(0, result.Length - 1);

        return result + "</" + nodeName + ">";
    }
}