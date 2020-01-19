using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI_detail_other : Poll_UI
{
    [SerializeField]
    Text description;

    Dropdown dropdown;

    private new void Awake()
    {
        base.Awake();
        dropdown = GetComponentInChildren<Dropdown>();
    }

    private void Start()
    {
        poll = Polls.Selected_Poll;
        colour_transparency = 0.5f;
        Set_Values(Polls.Selected_Poll);
    }

    new void Set_Values(Poll poll)
    {
        this.poll = poll;
        title.text = poll.Title;
        expiration_date.text = poll.Expiration_time;
        description.text = poll.Description;

        dropdown.ClearOptions();
        dropdown.AddOptions(poll.Vote_Types);
        dropdown.value = poll.Selected_Option_Idx;

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
        initialized = true;
    }



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

        string[] field_names = { "REQUEST_TYPE", "vote_poll_id", "vote_type" };
        string[] field_values = { "set_poll_vote", poll.Id.ToString(), poll.Vote_Types[vote_type] };
        Http_Client.Send_Post(field_names, field_values, Handle_Vote_Response);

        dropdown.interactable = false;
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    protected void Handle_Vote_Response(string response)
    {
        dropdown.interactable = true;

        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        poll = Polls.Parse_Single_Poll_Data(response);

        for (int x = 0; x < Polls.Poll_List.Count; x++)
        {
            if (Polls.Poll_List[x].Id == poll.Id)
            {
                Polls.Poll_List[x] = poll;
                break;
            }
        }

        Show_Details();
        Message.ShowMessage("Base de datos actualizada con éxito.");
    }
}
