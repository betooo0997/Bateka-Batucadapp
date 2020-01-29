using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events_section : MonoBehaviour
{
    public DateTime Date;

    [SerializeField]
    Text Title;

    public void Set_Value(DateTime date)
    {
        Date = new DateTime(date.Year, date.Month, 0);
        Title.text = Date.Month.ToString("MMMM") + Date.Year;
    }
}
