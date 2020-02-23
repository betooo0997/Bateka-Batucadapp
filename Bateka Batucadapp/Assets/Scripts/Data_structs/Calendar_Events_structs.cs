using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Calendar_Event : Votable
{
    public Calendar_Event() : base()
    {
        Location = "";
        Meeting_Location = "";
        Date = new DateTime();
        Meeting_Time = new DateTime(); ;
        Status = "";
    }

    public string Status;
    public string Location;
    public DateTime Date;
    public DateTime Meeting_Time;
    public string Meeting_Location;
}
