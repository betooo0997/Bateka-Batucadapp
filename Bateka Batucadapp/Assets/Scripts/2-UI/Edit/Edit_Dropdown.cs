#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Dropdown : MonoBehaviour
{
    public enum Content_Type
    {
        Year,
        Month,
        Day,
        Hour,
        Minute
    }

    public Content_Type Type;

    [SerializeField]
    Edit_Field_DateTime DateTime;

    [System.NonSerialized]
    public Dropdown Dropdown;

    public List<string> Options;

    bool intitialized;

    private void Awake()
    {
        Dropdown = GetComponent<Dropdown>();
    }

    public void Initialize()
    {
        Dropdown.options = new List<Dropdown.OptionData>();
        Options = new List<string>();
        DateTime data = (DateTime)DateTime.Info.GetValue(Edit_Handler.Data);
        uint value = 0;

        switch (Type)
        {
            case Content_Type.Year:
                value = (uint)data.Year - 2015;
                for (int x = 2015; x <= 2030; x++)
                    Options.Add(x.ToString());
                break;

            case Content_Type.Month:
                value = (uint)data.Month - 1;
                for (int x = 1; x <= 12; x++)
                    Options.Add(x.ToString());
                break;

            case Content_Type.Day:
                value = (uint)data.Day - 1;
                for (int x = 1; x <= 31; x++)
                    Options.Add(x.ToString());
                break;

            case Content_Type.Hour:
                value = (uint)data.Hour - 1;
                for (int x = 1; x < 24; x++)
                    Options.Add(x.ToString());
                break;

            case Content_Type.Minute:
                value = (uint)Mathf.RoundToInt((float)data.Minute / 5);
                for (int x = 0; x < 60; x += 5)
                    Options.Add(x.ToString());
                break;
        }

        for (int x = 0; x < Options.Count; x++)
            if (Options[x].Length < 2)
                Options[x] = "0" + Options[x];

        Dropdown.AddOptions(Options);
        Dropdown.value = (int)value;
        intitialized = true;
    }

    public void On_Value_Change(int data)
    {
        if(intitialized)
            DateTime.Update_Date();
    }
}
