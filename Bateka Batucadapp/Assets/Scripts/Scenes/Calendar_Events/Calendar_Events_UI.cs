using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Calendar_Events_UI : Data_UI
{
    public Text Title;
    public Text Location;

    public Text Meeting_Location;

    protected Calendar_Event calendar_event;

    public void Show_Details()
    {
        Calendar_Events.Selected_Data = calendar_event;
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Events_details);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("event"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }
}
