using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class Poll_UI : Data_UI
{
    [SerializeField]
    protected Text title;

    [SerializeField]
    protected Text expiration_date;

    [SerializeField]
    protected Poll poll;

    public void Show_Poll_Details()
    {
        Polls.Selected_Data = poll;
        Menu.Singleton.Load_Scene_Menu_Item((Menu.Menu_item)poll.Votable_Type);

        for (int x = 0; x < SceneManager.sceneCount; x++)
        {
            if (SceneManager.GetSceneAt(x).name.ToLower().Contains("poll"))
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(x));
                break;
            }
        }
    }
}
