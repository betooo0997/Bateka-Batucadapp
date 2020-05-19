using UnityEngine.UI;
using UnityEngine;

public class Poll_UI_detail_other : Poll_UI_detail
{
    Dropdown dropdown;

    [SerializeField]
    GameObject real_label;

    [SerializeField]
    GameObject false_label;


    void Awake()
    {
        dropdown = GetComponentInChildren<Dropdown>();
    }

    protected override void Initialize()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(poll.Vote_Types);

        if (poll.Selected_Option_Idx != -1)
        {
            real_label.SetActive(true);
            false_label.SetActive(false);
            dropdown.value = poll.Selected_Option_Idx;
        }

        base.Initialize();
    }

    protected override void Set_Interactable(bool interactable)
    {
        dropdown.interactable = interactable;
    }
}
