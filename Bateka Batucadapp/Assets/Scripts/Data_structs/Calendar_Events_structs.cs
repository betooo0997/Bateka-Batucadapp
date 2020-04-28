using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Calendar_Event : Votable
{
    public Calendar_Event() : base()
    {
        Votable_Type = Votable_Type.Yes_No;
        Location_Event = "";
        Location_Meeting = "";
        Date_Event = new DateTime();
        Date_Meeting = new DateTime(); ;
        Status = "";
    }

    public string Status;
    public string Location_Event;
    public DateTime Date_Event;
    public DateTime Date_Meeting;
    public string Location_Meeting;
}
