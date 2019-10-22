using UnityEngine;
using UnityEngine.SceneManagement;

public class Load_Scene : MonoBehaviour
{
    public static Load_Scene Sinlgeton;

    void Awake()
    {
        Sinlgeton = this;
    }

    public static void LoadScene(string scene_name)
    {
        SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
    }
}
