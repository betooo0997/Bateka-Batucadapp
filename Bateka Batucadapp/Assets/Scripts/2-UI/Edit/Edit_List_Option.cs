#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Edit_List_Option : MonoBehaviour
{
    Edit_Field_List edit_field_list;

    private void Awake()
    {
        edit_field_list = transform.GetComponentInParent<Edit_Field_List>();
    }

    public void Change_Name(string name)
    {
        edit_field_list.Change_Name(name, transform.GetSiblingIndex());
    }
}
