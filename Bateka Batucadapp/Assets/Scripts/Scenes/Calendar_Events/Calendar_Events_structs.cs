using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Calendar_Event : Data_struct
{
    public Calendar_Event()
    {
        Location = "";
        Meeting_Location = "";
        Date = new DateTime();
        Confirm_Deadline = new DateTime();
        Meeting_Time = new DateTime(); ;
        Vote_Voters = new List<List<User.User_Information>>();
        Vote_Types = new List<string>();
        Status = "";
    }

    public string Status;
    public string Location;
    public DateTime Date;
    public DateTime Confirm_Deadline;
    public DateTime Meeting_Time;
    public string Meeting_Location;
    public List<List<User.User_Information>> Vote_Voters;
    public List<string> Vote_Types;
}
