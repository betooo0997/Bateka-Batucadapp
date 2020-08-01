#pragma warning disable 0649

using System;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI_summarized : Poll_UI
{
    [SerializeField]
    Image arrow;

    public override void Set_Data(Data_struct poll)
    {
        this.poll = (Poll)poll;
        title.text = this.poll.Title.ToUpper();

        expiration_date.text = "FINALIZA " + this.poll.Date_Deadline.ToString("dd/MM/yyyy HH:mm") + "H";

        if (this.poll.Status != "" || this.poll.Is_Past_Deadline())
        {
            arrow.sprite = Helper.Singleton.Sprite_Arrow_Seen;

            if(this.poll.Is_Past_Deadline())
            {
                title.color = color_palette_gray(1);
                expiration_date.color = color_palette_light_gray(1);
            }
        }
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

        if (poll.Is_Past_Deadline())
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);
    }
}
