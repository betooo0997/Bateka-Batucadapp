using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Field_Enum : Edit_Field
{
    [SerializeField]
    Dropdown input_field;

    public override void Initialize(FieldInfo info)
    {
        base.Initialize(info);

        int x = 0;
        foreach (Enum state in Enum.GetValues(Info.FieldType))
        {
            if (state == Info.GetValue(Edit_Handler.Data))
                break;

            x++;
        }

        foreach (Enum state in Enum.GetValues(Info.FieldType))
            input_field.options.Add(new Dropdown.OptionData(state.ToString()));

        input_field.value = x;
        initialized = true;
    }

    public void On_End_Edit(int data)
    { 
        if (initialized)
        {
            int x = 0;

            foreach (Enum state in Enum.GetValues(Info.FieldType))
            {
                if (data == x)
                    break;

                x++;
            }

            object enumerable = Enum.GetValues(Info.FieldType).GetValue(x);
            Info.SetValue(Edit_Handler.Data, enumerable);
        }
    }
}
