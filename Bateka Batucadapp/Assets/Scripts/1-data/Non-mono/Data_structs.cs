using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class Data_struct
{
    protected List<string> editable;
    public uint Id;
    public string Title;
    public string Details;
    public uint Author_Id;
    public Privacy Privacy;

    public Data_struct()
    {
        editable = new List<string>();
        Title = "";
        Details = "";

        editable.Add("Title");
        editable.Add("Details");
        editable.Add("Privacy");
    }

    public bool Is_Editable(FieldInfo info)
    {
        return editable.Exists(a => a.ToLower() == info.Name.ToLower());
    }

    public Data_struct Deep_Copy()
    {
        Data_struct clone = (Data_struct)Activator.CreateInstance(GetType());

        FieldInfo[] infos = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo info in infos)
            info.SetValue(clone, info.GetValue(this));

        return clone;
    }

    public Type Database_Handler_Type()
    {
        if (GetType().Equals(typeof(Calendar_Event)))
            return typeof(Calendar_Events);

        else if (GetType().Equals(typeof(Poll)))
            return typeof(Polls);

        else
            return typeof(News);
    }
}

public enum Privacy
{
    Secret,
    Private,
    Public
}
