using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Events_UI_detail : Calendar_Events_UI
{
    public Text Detail;

    [SerializeField]
    Text confirm_deadline;

    [SerializeField]
    Image affirme;

    [SerializeField]
    Image reject;

    bool initialized;

    Button[] buttons;

    protected void Start()
    {
        calendar_event = (Calendar_Event)Calendar_Events.Selected_Data;
        buttons = GetComponentsInChildren<Button>();
        Initialize();
    }

    protected virtual void Initialize()
    {
        Title.text = calendar_event.Title;
        Meeting_Location.text = calendar_event.Meeting_Location + ", " + Utils.Get_String(calendar_event.Meeting_Time);
        Location.text = calendar_event.Location + ", " + Utils.Get_String(calendar_event.Date);
        Detail.text = calendar_event.Details;
        confirm_deadline.text = "Fecha de cierre de encuesta: " + Utils.Get_String(calendar_event.Confirm_Deadline);

        switch (calendar_event.Status)
        {
            case "affirmation":
                affirme.color = color_affirmed(1);
                break;

            case "rejection":
                reject.color = color_rejected(1);
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

        string[] field_names = { "REQUEST_TYPE", "vote_event_id", "vote_type" };
        string[] field_values = { "set_event_vote", calendar_event.Id.ToString(), calendar_event.Vote_Types[vote_type] };
        Http_Client.Send_Post(field_names, field_values, Handle_Event_Response);

        Set_Interactable(false);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    protected void Handle_Event_Response(string response, Handler_Type type)
    {
        Set_Interactable(true);

        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        calendar_event = Calendar_Events.Parse_Single_Data(response);

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

        Show_Event_Details();
        Message.ShowMessage("Base de datos actualizada con éxito.");
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
}
