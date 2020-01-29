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

    protected Color color_not_answered(float a) { return new Color(1f, 0.83f, 0f, a); }
    protected Color color_affirmed(float a) { return new Color(0.5f, 1, 0.5f, a); }
    protected Color color_rejected(float a) { return new Color(1f, 0.5f, 0.5f, a); }
    protected Color color_other(float a) { return color_affirmed(a); }

    public void Show_Poll_Details()
    {
        Polls.Selected_Data = poll;
        Menu.Singleton.Load_Scene_Menu_Item((Menu.Menu_item)poll.Type);

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
