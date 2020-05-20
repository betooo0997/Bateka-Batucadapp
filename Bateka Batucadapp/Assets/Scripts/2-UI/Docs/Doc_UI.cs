using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Doc_UI : Data_UI
{
    [SerializeField]
    protected Text Title;

    [SerializeField]
    protected Text Date;

    [SerializeField]
    protected Text Subtitle;

    [SerializeField]
    protected Doc Doc;

    public void Show_Doc_Details()
    {
        Docs.Selected_Data = Doc;
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Docs_details);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("doc"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }
}
