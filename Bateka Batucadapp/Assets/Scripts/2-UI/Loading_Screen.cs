using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading_Screen : MonoBehaviour
{
    static Loading_Screen singleton;
    static Image background;
    static float target_alpha;

    private void Awake()
    {
        singleton = this;
        background = GetComponent<Image>();

        if(target_alpha > 0)
            background.color = new Color(background.color.r, background.color.g, background.color.b, target_alpha);
    }

    public static void Set_Active(bool active, float alpha = 0)
    {
        if (active)
        {
            SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
            target_alpha = alpha;
        }
        else if (!active)
        {
            List<Scene> scenes = new List<Scene>();

            for (int x = 0; x < SceneManager.sceneCount; x++)
                if (SceneManager.GetSceneAt(x).name == "Loading")
                    scenes.Add(SceneManager.GetSceneAt(x));

            foreach (Scene scene in scenes)
                Utils.Singleton.StartCoroutine(Delayed_Unloading(scene));
        }
    }

    static IEnumerator Delayed_Unloading(Scene scene)
    {
        yield return new WaitForSeconds(0.25f);
        SceneManager.UnloadSceneAsync(scene);
    }
}
