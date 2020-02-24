using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading_Screen : MonoBehaviour
{
    static Loading_Screen singleton;

    private void Awake()
    {
        singleton = this;
    }

    public static void Set_Active(bool active)
    {
        if (active)
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        else if (!active)
        {
            List<Scene> scenes = new List<Scene>();

            for (int x = 0; x < SceneManager.sceneCount; x++)
                if (SceneManager.GetSceneAt(x).name == "Loading")
                    scenes.Add(SceneManager.GetSceneAt(x));

            foreach (Scene scene in scenes)
                SceneManager.UnloadSceneAsync(scene);
        }
    }
}
