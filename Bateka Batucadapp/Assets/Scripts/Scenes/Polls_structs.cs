using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Poll
{
    public Poll_Type Type;
    public string Status;
    public int Selected_Option_Idx;
    public uint Id;
    public string Title;
    public string Subtitle;
    public string Description;
    public string Creation_time;
    public string Expiration_time;
    public string Author;
    public string Privacy;
    public List<List<User.User_Information>> Vote_Voters;
    public List<string> Vote_Types;
    public List<Comment> Comments;

    public  string Convert_To_String()
    {
        string result = "<?xml version=" + '"' + "1.0" + '"' + "encoding=" + '"' + "utf - 8" + '"' + "?><poll>";

        result += "<id>" + Id.ToString() + @"</id>";
        result += "<title>" + Title + @"</title>";
        result += "<subtitle>" + Subtitle + @"</subtitle>";
        result += "<description>" + Description + @"</description>";
        result += "<creation_time>" + Creation_time + @"</creation_time>";
        result += "<expiration_time>" + Expiration_time + @"</expiration_time>";
        result += "<author>" + Author + @"</author>";
        result += "<privacy>" + Privacy + @"</privacy>";

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

    public Poll()
    {
        Creation_time = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm") + "h";
    }
}

public enum Poll_Type
{
    Yes_No,
    Other
}

[System.Serializable]
public struct Comment
{
    public string Author;
    public string Content;
}