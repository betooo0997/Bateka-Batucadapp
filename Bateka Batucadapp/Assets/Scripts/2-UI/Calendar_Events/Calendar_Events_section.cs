﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events_section : MonoBehaviour
{
    public DateTime Date;

    [SerializeField]
    Text Title = null;

    public void Set_Value(DateTime date)
    {
        Date = new DateTime(date.Year, date.Month, 1);
        Title.text = Date.ToString("MMMM").ToUpper() + "  ";
        Utils.Update_UI = true;
    }

    public static void Spawn_Sections()
    {
        List<DateTime> sections = new List<DateTime>();
        List<int> idxs = new List<int>();
        List<Data_struct> list = Database_Handler.Data_List_Get(typeof(Calendar_Events));

        for (int a = 0; a < list.Count; a++)
        {
            Calendar_Event c_event = (Calendar_Event)list[a];

            if (sections.Find(x => (x.Month == c_event.Date_Event.Month && x.Year == c_event.Date_Event.Year)).Year == 1)
            {
                sections.Add(new DateTime(c_event.Date_Event.Year, c_event.Date_Event.Month, 1));
                idxs.Add(a + idxs.Count);
            }
        }

        Transform parent = Calendar_Events.Singleton.Data_UI_Parent;

        for (int x = 0; x < idxs.Count; x++)
        {
            GameObject element_obj = Instantiate(((Calendar_Events)Calendar_Events.Singleton).Data_Section_UI_Prefab, parent);
            element_obj.name = typeof(Calendar_Events).ToString() + "_section";
            element_obj.transform.SetSiblingIndex(idxs[x] + 1);
            element_obj.GetComponent<Calendar_Events_section>().Set_Value(sections[x]);
        }
    }
}
