using System.Collections;
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
        if (additive) Utils.Singleton.StartCoroutine(Load_Scene_IE(scene_name));
        else SceneManager.LoadScene(scene_name, LoadSceneMode.Single);
    }

    static IEnumerator Load_Scene_IE(string scene_name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(scene_name, LoadSceneMode.Additive);
        yield return null;
        //async.allowSceneActivation = false;

        //while (async.progress <= 0.89f)
            //yield return null;

        //async.allowSceneActivation = true;
    }
}
