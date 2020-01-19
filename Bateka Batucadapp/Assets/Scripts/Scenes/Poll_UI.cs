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

    [SerializeField]
    protected Poll poll;

    protected Image background;

    protected float colour_transparency = 1f;
    protected Color color_not_answered() { return new Color(1f, 0.83f, 0f, colour_transparency); }
    protected Color color_affirmed() { return new Color(0.5f, 1, 0.5f, colour_transparency); }
    protected Color color_rejected() { return new Color(1f, 0.5f, 0.5f, colour_transparency); }
    protected Color color_other() { return color_affirmed(); }

    protected bool initialized;

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
        switch(poll.Status)
        {
            case "affirmation":
                image.color = color_affirmed();
                break;

            case "rejection":
                image.color = color_rejected();
                break;

            case "":
                image.color = color_not_answered();
                break;

            default:
                image.color = color_other();
                break;
        }
    }

    public void Show_Details()
    {
        Polls.Selected_Poll = poll;
        Scene scene = SceneManager.GetSceneByName("Polls");

        if(!scene.IsValid())
            scene = SceneManager.GetSceneByName("Poll_details_yes_no");

        if (!scene.IsValid())
            scene = SceneManager.GetSceneByName("Poll_details_other");

        if(poll.Type == Poll_Type.Yes_No)
            Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Poll_details_yes_no);
        else
            Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Poll_details_other);

        if (scene.IsValid())
            SceneManager.UnloadSceneAsync(scene);
    }
}
