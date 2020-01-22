using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class News_UI : MonoBehaviour
{
    public Text Title;
    public Text Creation_time;

    protected News_Entry news_entry;

    public void Show_News_Details()
    {
        News.Selected_News_Entry = news_entry;
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.News_details);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("news"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }
}
