using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Day : Data_UI
{
    public Calendar_Event calendar_event;

    [SerializeField]
    Image type;

    [SerializeField]
    Image status;

    [SerializeField]
    GameObject borders;

    static GameObject enabled_borders;

    Text day;

    private void Awake()
    {
        day = GetComponentInChildren<Text>();
        calendar_event = new Calendar_Event();
    }

    public override void Set_event(Data_struct data_struct)
    {
        if (data_struct == null)
            return;

        calendar_event = (Calendar_Event)data_struct;
        Update_Color(status);
    }

    public void Set_date(DateTime date)
    {
        day.text = date.Day.ToString();

        if (date.Month != DateTime.Now.Month)
            day.color = new Color(0.65f, 0.65f, 0.65f);
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

    public void Show_Details()
    {
        if(enabled_borders != null)
            enabled_borders.SetActive(false);

        borders.SetActive(true);
        enabled_borders = borders;

        if (calendar_event.Date.Year == 1)
            return;

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
