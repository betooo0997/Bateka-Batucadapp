#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Edit_Field_DateTime : Edit_Field
{
    public Edit_Dropdown Year, Month, Day, Hour, Minute;

    public override void Initialize(FieldInfo info)
    {
        base.Initialize(info);

        Year.Initialize();
        Month.Initialize();
        Day.Initialize();
        Hour.Initialize();
        Minute.Initialize();
    }

    public void Update_Date()
    {
        Info.SetValue(Edit_Handler.Data, new DateTime(
            int.Parse((Year.Options     [Year.Dropdown.value])),
            int.Parse((Month.Options    [Month.Dropdown.value])),
            int.Parse((Day.Options      [Day.Dropdown.value])),
            int.Parse((Hour.Options     [Hour.Dropdown.value])),
            int.Parse((Minute.Options   [Minute.Dropdown.value])),
            0
            ));
    }
}
