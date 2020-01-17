using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI : MonoBehaviour
{
    [SerializeField]
    Text title;
    [SerializeField]
    Text expiration_date;
    Image background;

    public uint Poll_id;
    public Poll_Type Poll_type;
    public Poll_Status Poll_status;

    static Color color_not_answered = new Color(1f, 0.83f, 0f);
    static Color color_affirmed = new Color(0.5f, 1, 0.5f);
    static Color color_rejected = new Color(1f, 0.5f, 0.5f);
    static Color color_other = color_affirmed;

    private void Awake()
    {
        background = GetComponent<Image>();
    }

    public void Set_Values(uint poll_id, string title, string expiration_date, Poll_Status poll_status, Poll_Type poll_type)
    {
        Poll_id = poll_id;
        this.title.text = title;
        this.expiration_date.text = expiration_date;
        Poll_status = poll_status;
        Update_Background();
    }

    void Update_Background()
    {
        switch(Poll_status)
        {
            case Poll_Status.Affirmed:
                background.color = color_affirmed;
                break;

            case Poll_Status.Rejected:
                background.color = color_rejected;
                break;

            case Poll_Status.Other:
                background.color = color_other; ;
                break;
        }
    }

    public void Show_Details()
    {

    }
}
