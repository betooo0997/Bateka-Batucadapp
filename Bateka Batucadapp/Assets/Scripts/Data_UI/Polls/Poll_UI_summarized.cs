using UnityEngine.UI;

public class Poll_UI_summarized : Poll_UI
{
    protected Image background;

    protected void Awake()
    {
        background = GetComponent<Image>();
    }

    public override void Set_event(Data_struct poll)
    {
        this.poll = (Poll)poll;
        title.text = this.poll.Title;
        expiration_date.text = this.poll.Expiration_time;
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
    }
}
