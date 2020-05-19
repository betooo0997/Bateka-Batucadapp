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
    public string Author;

    public Poll() : base()
    {
        Creation_Time = DateTime.Now;
    }

    public  string Convert_To_String()
    {
        string result = "<?xml version=" + '"' + "1.0" + '"' + "encoding=" + '"' + "utf - 8" + '"' + "?><poll>";

        result += "<id>" + Id.ToString() + @"</id>";
        result += "<title>" + Title + @"</title>";
        result += "<subtitle>" + Subtitle + @"</subtitle>";
        result += "<description>" + Details + @"</description>";
        result += "<creation_time>" + Creation_Time + @"</creation_time>";
        result += "<expiration_time>" + Date_Deadline + @"</expiration_time>";
        result += "<author>" + Author + @"</author>";
        result += "<privacy>" + Privacy.ToString() + @"</privacy>";

        for (int x = 0; x < Vote_Types.Count; x++)
            result += Get_VoteList(Vote_Voters[x], Vote_Types[x]);

        result += @"<comments></comments></poll>";

        return result;
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