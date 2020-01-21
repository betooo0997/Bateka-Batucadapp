using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poll_UI_detail_yes_no : Poll_UI_detail
{
    [SerializeField]
    Image affirme;

    [SerializeField]
    Image reject;

    Button[] buttons;

    private void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    protected override void Initialize()
    {
        switch (poll.Status)
        {
            case "affirmation":
                affirme.color = color_affirmed(1);
                break;

            case "rejection":
                reject.color = color_rejected(1);
                break;
        }

        base.Initialize();
    }

    protected override void Set_Interactable(bool interactable)
    {
        foreach (Button button in buttons)
            button.interactable = false;
    }
}
