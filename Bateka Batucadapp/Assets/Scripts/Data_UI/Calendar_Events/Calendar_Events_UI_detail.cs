using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Events_UI_detail : Calendar_Events_UI
{
    [SerializeField]
    Image affirme, reject;

    [SerializeField]
    Text date_event, transportation, cash, food, detail, date_deadline;

    Sprite affired_default, rejected_default;

    bool initialized;

    Button[] buttons;

    int temp_vote;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
        affired_default = affirme.sprite;
        rejected_default = reject.sprite;
    }

    protected void Start()
    {
        calendar_event = (Calendar_Event)Calendar_Events.Selected_Data;
        Initialize();
    }

    protected virtual void Initialize()
    {
        if (Utils.Is_Sooner(calendar_event.Date_Deadline, DateTime.Now))
            Set_Interactable(false);

        Title.text              = calendar_event.Title;
        date_event.text         = Utils.Get_String(calendar_event.Date_Event);
        Location_event.text     = calendar_event.Location_Event;
        Meeting.text            = Utils.Get_String(calendar_event.Date_Meeting) + ", " + calendar_event.Location_Meeting;
        transportation.text     = calendar_event.Transportation;
        cash.text               = calendar_event.Cash;
        food.text               = calendar_event.Food;
        detail.text             = calendar_event.Details;
        date_deadline.text      = Utils.Get_String(calendar_event.Date_Deadline);

        switch (calendar_event.Status)
        {
            case "affirmation":
                affirme.sprite = Helper.Singleton.Sprite_Event_Button_Affirmed;
                break;

            case "rejection":
                reject.sprite = Helper.Singleton.Sprite_Event_Button_Rejected;
                break;
        }

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
        initialized = true;
    }



    // ______________________________________
    //
    // VOTE ON EVENTS.
    // ______________________________________
    //


    /// <summary>
    /// Updates user's choice of vote locally on the device and remotely on the server.
    /// </summary>
    public void Vote(int vote_type)
    {
        if (!initialized) return;

        temp_vote = vote_type;

        string[] field_names = { "REQUEST_TYPE", "event_id", "event_response" };
        string[] field_values = { "set_event_vote", calendar_event.Id.ToString(), vote_type.ToString() };
        Http_Client.Send_Post(field_names, field_values, Handle_Event_Response);

        Set_Interactable(false);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    protected void Handle_Event_Response(string response, Handler_Type type)
    {
        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        if (User.User_Info.Events_Data.Exists(x => x.id == calendar_event.Id))
            User.User_Info.Events_Data.Find(x => x.id == calendar_event.Id).response = temp_vote;
        else
            User.User_Info.Events_Data.Add(new User.Vote_Data() { id = calendar_event.Id, response = temp_vote });

        for (int x = 0; x < calendar_event.Vote_Voters.Count; x++)
            if (calendar_event.Vote_Voters[x].Exists(a => a.Id == User.User_Info.Id))
                calendar_event.Vote_Voters[x].Remove(calendar_event.Vote_Voters[x].Find(a => a.Id == User.User_Info.Id));

        calendar_event.Vote_Voters[temp_vote].Add(User.User_Info);

        calendar_event.Status = calendar_event.Vote_Types[temp_vote];
        List<Data_struct> calendar_events = Calendar_Events.Data_List_Get(typeof(Calendar_Events));

        for (int x = 0; x < calendar_events.Count; x++)
        {
            if (calendar_events[x].Id == calendar_event.Id)
            {
                calendar_events[x] = calendar_event;
                Calendar_Events.Data_List_Set(typeof(Calendar_Events), calendar_events);
                break;
            }
        }

        DataSaver.Override_Database_entry(Handler_Type.events.ToString() + "_database", response, calendar_event.Id);
        Message.ShowMessage("Base de datos actualizada con éxito.");

        if (this != null)
        {
            Show_Event_Details();
            Set_Interactable(true);
        }
    }

    protected void Set_Interactable(bool interactable)
    {
        foreach (Button button in buttons)
            button.interactable = false;
    }

    public void Show_Event_Details()
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

    public void On_Back_Button()
    {
        if (Menu.Prev_Item == Menu.Menu_item.Home)
            Utils.Singleton.Load_Menu_Scene("Home");
        else
            Utils.Singleton.Load_Menu_Scene("Events");
    }
}
