using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_Scene : MonoBehaviour
{
    public static Load_Scene Singleton;

    void Awake()
    {
        Singleton = this;
    }

    public static void Load_Scene_ST(string scene_name, bool additive = true)
    {
        if (additive) SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        else SceneManager.LoadScene(scene_name, LoadSceneMode.Single);
    }
}
