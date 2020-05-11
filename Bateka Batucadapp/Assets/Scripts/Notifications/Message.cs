using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    Image image;
    Text text;

    static string content;
    static bool active;

    float timer;

    bool shown;

    void Start()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();
        text.text = content;
    }

    void Update()
    {
        if(!shown)
        {
            if (transform.localPosition.y > -75 - 43)
                transform.localPosition -= new Vector3(0, Time.deltaTime * 175);
            else
            {
                transform.localPosition = new Vector3(0, -75 - 43);
                timer += Time.deltaTime;

                if (timer > 1.5f)
                    shown = true;
            }
        }
        else
        {
            transform.localPosition += new Vector3(0, Time.deltaTime * 175);
            if (transform.localPosition.y > 100)
            {
                active = false;
                SceneManager.UnloadSceneAsync("Message");
            }
        }
    }

    static IEnumerator ShowMessageDelayed(string message)
    {
        yield return new WaitForSeconds(0.5f);
        ShowMessage(message);
    }

    public static void ShowMessage(string message)
    {
        if (active)
            Initializer.Singleton.StartCoroutine(ShowMessageDelayed(message));
        else
        {
            active = true;
            content = message;
            Load_Scene.Load_Scene_ST("Message");
        }
    }
}
