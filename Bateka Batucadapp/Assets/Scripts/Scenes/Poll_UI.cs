using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Poll_UI : MonoBehaviour
{
    [SerializeField]
    protected Text title;

    [SerializeField]
    protected Text expiration_date;

    protected Image background;

    [SerializeField]
    protected Poll poll;

    protected float colour_transparency = 1f;

    protected Color color_not_answered() { return new Color(1f, 0.83f, 0f, colour_transparency); }
    protected Color color_affirmed() { return new Color(0.5f, 1, 0.5f, colour_transparency); }
    protected Color color_rejected() { return new Color(1f, 0.5f, 0.5f, colour_transparency); }
    protected Color color_other() { return color_affirmed(); }

    protected void Awake()
    {
        background = GetComponent<Image>();
    }

    public void Set_Values(Poll poll)
    {
        this.poll = poll;
        title.text = poll.Title;
        expiration_date.text = poll.Expiration_time;
        Update_Background();
    }

    protected void Update_Background()
    {
        switch(poll.Status)
        {
            case Poll_Status.Affirmed:
                background.color = color_affirmed();
                break;

            case Poll_Status.Rejected:
                background.color = color_rejected();
                break;

            case Poll_Status.Other:
                background.color = color_other();
                break;
        }
    }

    public void Show_Details()
    {
        Polls.Selected_Poll = poll;
        Scene scene = SceneManager.GetSceneByName("Polls");

        if(!scene.IsValid())
            scene = SceneManager.GetSceneByName("Poll_details_yes_no");

        SceneManager.LoadScene("Poll_details_yes_no", LoadSceneMode.Additive);

        if (scene.IsValid())
            SceneManager.UnloadSceneAsync(scene.buildIndex);
    }
}
