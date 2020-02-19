using System;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI_summarized : Poll_UI
{
    [SerializeField]
    Image background;

    public override void Set_Event(Data_struct poll)
    {
        this.poll = (Poll)poll;
        title.text = this.poll.Title;

        expiration_date.text = this.poll.Expiration_time.ToString("dd/MM/yyyy | HH:mm") + "h";
        Update_Color(background);
    }

    protected void Update_Color(Image image)
    {
        switch (poll.Status)
        {
            case "affirmation":
                image.color = color_affirmed(1);
                break;

            case "rejection":
                image.color = color_rejected(1);
                break;

            case "":
                image.color = color_not_answered(1);
                break;

            default:
                image.color = color_other(1);
                break;
        }

        if (Utils.Is_Sooner(poll.Expiration_time, DateTime.Now))
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);
    }
}
