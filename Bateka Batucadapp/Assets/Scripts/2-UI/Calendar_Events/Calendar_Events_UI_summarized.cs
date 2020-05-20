using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events_UI_summarized : Calendar_Events_UI
{
    [SerializeField]
    Text day = null;

    [SerializeField]
    Text month = null;

    [SerializeField]
    Image background = null;

    protected void Awake()
    {
        if(background == null)
            background = GetComponent<Image>();
    }

    public override void Set_Data(Data_struct calendar_event)
    {
        this.calendar_event = (Calendar_Event)calendar_event;
        day.text = this.calendar_event.Date_Event.Day.ToString();
        Title.text = this.calendar_event.Title;
        month.text = this.calendar_event.Date_Event.ToString("MMMM").Substring(0, 3);
        Meeting.text = this.calendar_event.Location_Meeting + ", " + Utils.Get_String(this.calendar_event.Date_Meeting);
        Location_event.text = this.calendar_event.Location_Event + ", " + Utils.Get_String(this.calendar_event.Date_Event);
        Update_Color(background);
    }

    protected void Update_Color(Image image)
    {
        switch (calendar_event.Status)
        {
            case "affirmation":
                image.sprite = Helper.Singleton.Sprite_Event_Affirmed;
                break;

            case "rejection":
                image.sprite = Helper.Singleton.Sprite_Event_Rejected;
                break;

            default:
                image.sprite = Helper.Singleton.Sprite_Event_Not_Answered;
                break;
        }
    }
}
