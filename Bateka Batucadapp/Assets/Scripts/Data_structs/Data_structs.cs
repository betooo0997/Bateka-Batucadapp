using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Data_struct
{
    public uint Id;
    public string Title;
    public string Details;
}

public enum Privacy
{
    Secret,
    Private,
    Public
}
