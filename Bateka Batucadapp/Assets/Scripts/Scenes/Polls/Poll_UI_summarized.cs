using UnityEngine.UI;

public class Poll_UI_summarized : Poll_UI
{
    protected Image background;

    protected void Awake()
    {
        background = GetComponent<Image>();
    }

    public void Set_Values(Poll poll)
    {
        this.poll = poll;
        title.text = poll.Title;
        expiration_date.text = poll.Expiration_time;
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
