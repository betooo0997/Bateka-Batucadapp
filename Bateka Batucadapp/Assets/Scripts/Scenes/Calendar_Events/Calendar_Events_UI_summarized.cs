using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar_Events_UI_summarized : Calendar_Events_UI
{
    public void Set_Values(Calendar_Event calendar_event)
    {
        this.calendar_event = calendar_event;
        Title.text = calendar_event.Title;
        Date.text = Utils.Get_String(calendar_event.Date);
    }
}
