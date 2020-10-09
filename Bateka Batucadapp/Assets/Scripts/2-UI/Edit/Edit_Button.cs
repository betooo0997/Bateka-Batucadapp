using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_Button : MonoBehaviour
{
    public void Set_Edit_Handler_Data(bool empty)
    {
        if(empty)
        {
            switch(Menu.Active_Item)
            {
                case Menu.Menu_item.Polls:
                    Edit_Handler.Data = (Data_struct)Activator.CreateInstance(typeof(Poll));
                    return;
                case Menu.Menu_item.News:
                    Edit_Handler.Data = (Data_struct)Activator.CreateInstance(typeof(News_Entry));
                    return;
                case Menu.Menu_item.Events:
                    Edit_Handler.Data = (Data_struct)Activator.CreateInstance(typeof(Calendar_Event));
                    return;
            }
        }

        Edit_Handler.Data = Database_Handler.Selected_Data.Clone_Deep();
    }
}
