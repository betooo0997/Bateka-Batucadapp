using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Day : Data_UI
{
    public List<Calendar_Event> Calendar_events;

    [SerializeField]
    Image type;

    [SerializeField]
    Image status;

    [SerializeField]
    GameObject borders;

    static GameObject enabled_borders;

    DateTime date;

    Text day;

    private void Awake()
    {
        day = GetComponentInChildren<Text>();
    }

    public void Set_Event(List<Data_struct> data_struct)
    {
        if (data_struct == null)
            return;

        Calendar_events = data_struct.Cast<Calendar_Event>().ToList();

        if (Calendar_events.Count > 0)
            Update_Color(status);
        else if (date.Day == DateTime.Now.Day && date.Month == DateTime.Now.Month && date.Year == DateTime.Now.Year)
        {
            status.color = new Color(0.9f, 0.9f, 0.9f);

            if (enabled_borders != null)
                enabled_borders.SetActive(false);

            borders.SetActive(true);
            enabled_borders = borders;
        }
    }

    public void Set_date(DateTime date)
    {
        this.date = date;
        day.text = date.Day.ToString();

        if (date.Month != DateTime.Now.Month)
            day.color = new Color(0.65f, 0.65f, 0.65f);
    }

    protected void Update_Color(Image image)
    {
        switch (Calendar_events[0].Status)
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
        if (enabled_borders != null)
            enabled_borders.SetActive(false);

        borders.SetActive(true);
        enabled_borders = borders;

        Calendar_Handler.Singleton.Show_Overview(this);
    }

    /*
    public void Show_Details()
    {
        if(enabled_borders != null)
            enabled_borders.SetActive(false);

        borders.SetActive(true);
        enabled_borders = borders;

        if (Calendar_events.Date.Year == 1)
            return;

        Calendar_Events.Selected_Data = Calendar_events;
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Events_details);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("event"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }*/
}
