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
    public int Author_Id;
    public Privacy Privacy;

    public Data_struct()
    {
        editable = new List<string>();
        Title = "";
        Details = "";

        editable.Add("Title");
        editable.Add("Details");
    }

    public bool Is_Editable(FieldInfo info)
    {
        return editable.Exists(a => a.ToLower() == info.Name.ToLower());
    }
}

public enum Privacy
{
    Secret,
    Private,
    Public
}
