#pragma warning disable 0649

using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Field_List : Edit_Field
{
    [SerializeField]
    List<InputField> options;

    [SerializeField]
    Transform options_parent;

    [SerializeField]
    GameObject option_prefab;

    [SerializeField]
    Transform add_options_button;

    public override void Initialize(FieldInfo info)
    {
        base.Initialize(info);

        foreach(string element in (List<string>)Info.GetValue(Edit_Handler.Data))
            Add_Option(element);

        initialized = true;
    }

    public void Change_Name(string name, int idx)
    {
        List<string> vote_types = (List<string>)Info.GetValue(Edit_Handler.Data);
        vote_types[idx] = name;
        Info.SetValue(Edit_Handler.Data, vote_types);
        Enable_Save_Button();
    }

    public void Add_Option(string name)
    {
        List<string> vote_types = (List<string>)Info.GetValue(Edit_Handler.Data);

        GameObject new_object = Instantiate(option_prefab, options_parent);
        new_object.name = "Option " + name;
        new_object.GetComponent<InputField>().text = name;
        new_object.transform.SetAsLastSibling();
        add_options_button.SetAsLastSibling();

        if (initialized)
        {
            vote_types.Add(new_object.name);
            Info.SetValue(Edit_Handler.Data, vote_types);
            Enable_Save_Button();
        }

        Debug.Log("aaasa");

        Utils.Update_UI = true;
    }
}
