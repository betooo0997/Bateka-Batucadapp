using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Poll_UI_detail : Poll_UI
{
    [SerializeField]
    Text description;

    protected bool initialized;

    int temp_vote;

    protected void Start()
    {
        poll = (Poll)Polls.Selected_Data;
        Initialize();
    }

    protected virtual void Initialize()
    {
        title.text = poll.Title;
        expiration_date.text = poll.Date_Deadline.ToString("dd/MM/yyyy | HH:mm") + "h";
        description.text = poll.Details;

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
        initialized = true;

        if (Utils.Is_Sooner(poll.Date_Deadline, DateTime.Now))
            Set_Interactable(false);
    }

    protected abstract void Set_Interactable(bool interactable);



    // ______________________________________
    //
    // VOTE ON POLLS.
    // ______________________________________
    //


    /// <summary>
    /// Updates user's choice of vote locally on the device and remotely on the server.
    /// </summary>
    public void Vote(int vote_type)
    {
        if (!initialized) return;

        temp_vote = vote_type;

        string[] field_names = { "REQUEST_TYPE", "poll_id", "poll_response" };
        string[] field_values = { "set_poll_vote", poll.Id.ToString(), vote_type.ToString() };
        Http_Client.Send_Post(field_names, field_values, Handle_Vote_Response);

        Set_Interactable(false);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    protected void Handle_Vote_Response(string response, Handler_Type type)
    {
        Set_Interactable(true);

        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        User.User_Info.Polls_Data.Find(x => x.id == poll.Id).response = temp_vote;

        poll.Status = poll.Vote_Types[temp_vote];
        poll.Selected_Option_Idx = temp_vote;
        List<Data_struct> polls = Polls.Data_List_Get(typeof(Polls));

        for (int x = 0; x < polls.Count; x++)
        {
            if (polls[x].Id == poll.Id)
            {
                polls[x] = poll;
                Polls.Data_List_Set(typeof(Polls), polls);
                break;
            }
        }

        DataSaver.Override_Database_entry(Handler_Type.polls.ToString() + "_database", response, poll.Id);

        Show_Poll_Details();
        Message.ShowMessage("Base de datos actualizada con éxito.");
    }
}
