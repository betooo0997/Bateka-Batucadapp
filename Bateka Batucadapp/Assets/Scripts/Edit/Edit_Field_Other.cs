using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Field_Other : Edit_Field
{
    [SerializeField]
    InputField input_field;

    bool changing;

    string prev;

    public override void Initialize(FieldInfo info)
    {
        base.Initialize(info);

        if (Info.FieldType == typeof(DateTime))
            input_field.text = Utils.Get_String_SQL((DateTime)Info.GetValue(Edit_Handler.Data));
        else
            input_field.text = Info.GetValue(Edit_Handler.Data).ToString();

        prev = input_field.text;

        initialized = true;
    }

    string Correct_Date_Time(string data)
    {
        changing = true;

        int caret_pos = input_field.caretPosition;

        if (data.Length == 15)
        {
            if (caret_pos > 0)
            {
                if(caret_pos == 15)
                    data = data.Substring(0, caret_pos) + '0';
                else
                    data = data.Substring(0, caret_pos) + '0' + data.Substring(caret_pos);
            }
            else
                data = '0' + data;
        }

        if(data.Length == 17)
        {
            if (caret_pos >= 16)
                data = data.Substring(0, 16);
            else
                data = data.Substring(0, caret_pos) + data.Substring(caret_pos + 1);
        }

        bool Check_If_Digit(int[] idxs, string elements)
        {
            foreach (int idx in idxs)
                if (!char.IsDigit(elements[idx]))
                    return false;

            return true;
        }

        if (data.Length != 16 || !Check_If_Digit(new int[] { 0, 1, 2, 3, 5, 6, 8, 9, 11, 12, 14, 15 }, data)
            || data[4] != '-' || data[7] != '-' || data[10] != ' ' || data[13] != ':')
            input_field.text = prev;
        else
        {
            input_field.text = data;
            prev = data;
        }

        changing = false;
        return prev;
    }

    public void On_Value_Change(string data)
    {
        if (initialized && !changing && Info.FieldType == typeof(DateTime))
            Correct_Date_Time(data);
    }

    public void On_End_Edit(string data)
    {
        if (initialized && !changing)
        {
            object value = data;

            if (Info.FieldType == typeof(DateTime))
                value = Utils.Get_DateTime(Correct_Date_Time(data));

            Info.SetValue(Edit_Handler.Data, value);
        }
    }
}
