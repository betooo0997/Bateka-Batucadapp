#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Field_Text : Edit_Field
{
    [SerializeField]
    InputField input_field;
    
    public override void Initialize(FieldInfo info)
    {
        base.Initialize(info);
        input_field.text = Info.GetValue(Edit_Handler.Data).ToString();

        string type = info.FieldType.ToString().ToLower();

        if (type == "int" || type == "uint")
            input_field.contentType = InputField.ContentType.IntegerNumber;
        else if (type == "float" || type == "double")
            input_field.contentType = InputField.ContentType.DecimalNumber;

        initialized = true;
    }

    public void On_End_Edit(string data)
    {
        if (initialized)
            Info.SetValue(Edit_Handler.Data, data);
    }
}
