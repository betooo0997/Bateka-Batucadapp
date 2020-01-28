using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events_UI_detail : Calendar_Events_UI
{
    public Text Detail;

    [SerializeField]
    Text confirm_deadline;


    protected void Start()
    {
        calendar_event = Calendar_Events.Selected_Event;
        Initialize();
    }

    protected virtual void Initialize()
    {
        Title.text = calendar_event.Title;
        Date.text = Utils.Get_String(calendar_event.Date);
        confirm_deadline.text = Utils.Get_String(calendar_event.Confirm_Deadline);
        Detail.text = calendar_event.Details;

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
    }
}
