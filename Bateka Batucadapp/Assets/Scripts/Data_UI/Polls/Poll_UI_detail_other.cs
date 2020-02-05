using UnityEngine.UI;

public class Poll_UI_detail_other : Poll_UI_detail
{
    Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponentInChildren<Dropdown>();
    }

    protected override void Initialize()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(poll.Vote_Types);
        dropdown.value = poll.Selected_Option_Idx;

        base.Initialize();
    }

    protected override void Set_Interactable(bool interactable)
    {
        dropdown.interactable = interactable;
    }
}
