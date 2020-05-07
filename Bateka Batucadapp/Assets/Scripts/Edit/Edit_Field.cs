using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Field : MonoBehaviour
{
    public FieldInfo Info;

    [SerializeField]
    protected Text title;

    protected bool initialized;

    public virtual void Initialize(FieldInfo info)
    {
        Info = info;
        title.text = Info.Name;
    }
}
