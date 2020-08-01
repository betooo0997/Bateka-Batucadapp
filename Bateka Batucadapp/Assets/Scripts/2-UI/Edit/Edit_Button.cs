using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edit_Button : MonoBehaviour
{
    public void Set_Edit_Handler_Data()
    {
        Edit_Handler.Data = Database_Handler.Selected_Data;
    }
}
