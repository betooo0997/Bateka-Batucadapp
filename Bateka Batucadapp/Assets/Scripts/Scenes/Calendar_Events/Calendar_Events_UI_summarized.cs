using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar_Events_UI_summarized : Calendar_Events_UI
{
    public override void Set_Values(Data_struct calendar_event)
    {
        this.calendar_event = (Calendar_Event)calendar_event;
        Title.text = this.calendar_event.Title;
        Date.text = Utils.Get_String(this.calendar_event.Date);
    }
}
