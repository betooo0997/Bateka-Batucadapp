using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events_UI_summarized : Calendar_Events_UI
{
    [SerializeField]
    Text day;

    [SerializeField]
    Text month;

    protected Image background;

    protected void Awake()
    {
        background = GetComponent<Image>();
    }

    public override void Set_Event(Data_struct calendar_event)
    {
        this.calendar_event = (Calendar_Event)calendar_event;
        day.text = this.calendar_event.Date.Day.ToString();
        Title.text = this.calendar_event.Title;
        month.text = this.calendar_event.Date.ToString("MMMM").Substring(0, 3);
        Meeting_Location.text = this.calendar_event.Meeting_Location + ", " + Utils.Get_String(this.calendar_event.Meeting_Time);
        Location.text = this.calendar_event.Location + ", " + Utils.Get_String(this.calendar_event.Date);
        Update_Color(background);
    }

    protected void Update_Color(Image image)
    {
        switch (calendar_event.Status)
        {
            case "affirmation":
                image.color = color_affirmed(1);
                break;

            case "rejection":
                image.color = color_rejected(1);
                break;

            default:
                image.color = color_not_answered(1);
                break;
        }
    }
}
