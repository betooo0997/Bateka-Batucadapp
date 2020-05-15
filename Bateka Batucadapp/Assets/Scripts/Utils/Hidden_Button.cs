using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Hidden_Button : MonoBehaviour
{
    [SerializeField]
    int necessary_clicks;

    [SerializeField]
    string scene_to_load;

    int clicks = 0;

    public void On_Cick()
    {
        clicks += 1;

        if(clicks >= necessary_clicks)
        {
            clicks = 0;
            Utils.Singleton.Load_Menu_Scene(scene_to_load);
        }
    }
}
