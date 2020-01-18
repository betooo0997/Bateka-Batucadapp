using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI_detail_yes_no : Poll_UI
{
    [SerializeField]
    Image affirme;

    [SerializeField]
    Image reject;

    [SerializeField]
    Text description;

    Button[] buttons;

    private new void Awake()
    {
        base.Awake();
        buttons = GetComponentsInChildren<Button>();
    }

    private void Start()
    {
        poll = Polls.Selected_Poll;
        colour_transparency = 0.5f;
        Set_Values(Polls.Selected_Poll);
    }

    new void Set_Values(Poll poll)
    {
        base.Set_Values(poll);
        description.text = poll.Description;

        if(poll.Type == Poll_Type.Yes_No)
        {
            switch(poll.Status)
            {
                case Poll_Status.Affirmed:
                    affirme.color = new Color(color_affirmed().r, color_affirmed().g, color_affirmed().b, 1);
                    break;

                case Poll_Status.Rejected:
                    reject.color = new Color(color_rejected().r, color_rejected().g, color_rejected().b, 1);
                    break;
            }
        }

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
    }



    // ______________________________________
    //
    // VOTE ON POLLS.
    // ______________________________________
    //


    /// <summary>
    /// Updates user's choice of vote locally on the device and remotely on the server.
    /// </summary>
    public void Vote(string vote_type)
    {
        string[] field_names = { "REQUEST_TYPE", "vote_poll_id", "vote_type" };
        string[] field_values = { "set_poll_vote", poll.Id.ToString(), vote_type };
        Http_Client.Send_Post(field_names, field_values, Handle_Vote_Response);

        foreach (Button button in buttons)
            button.interactable = false;
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    protected void Handle_Vote_Response(string response)
    {
        foreach (Button button in buttons)
            button.interactable = true;

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
