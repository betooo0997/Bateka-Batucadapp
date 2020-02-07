using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Overview : Data_UI
{
    [SerializeField]
    Text title;

    [SerializeField]
    Text detail;

    Image image;

    Calendar_Event calendar_event;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetValues(Calendar_Event calendar_event)
    {
        this.calendar_event = calendar_event;
        title.text = calendar_event.Title;
        detail.text = calendar_event.Details;

        Update_Color(image);
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

    public void See_Details()
    {
        Calendar_Events.Selected_Data = calendar_event;
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Events_details);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("event"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }
}
