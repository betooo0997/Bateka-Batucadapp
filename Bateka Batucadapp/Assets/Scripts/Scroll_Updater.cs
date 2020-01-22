using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll_Updater : MonoBehaviour
{
    public static Scroll_Updater Singleton;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    RectTransform Spacer;

    [SerializeField]
    Image icon;

    Image image;

    bool updating;
    bool initialized;

    private void Awake()
    {
        Singleton = this;
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!initialized || (!updating && rectTransform.localPosition.y < -50 && Input.GetMouseButtonUp(0))) // TODO: Add touch!!
        {
            switch (Menu.Singleton.active_menu_item)
            {
                case Menu.Menu_item.News:
                    News.Load_Data_Server();
                    break;

                case Menu.Menu_item.Polls:
                    Polls.Load_Data_Server();
                    break;
            }

            updating = true;
            image.enabled = true;
            icon.enabled = true;
            initialized = true;
            Spacer.sizeDelta = new Vector3(Spacer.sizeDelta.x, 34);
            Canvas.ForceUpdateCanvases();
            GameObject.FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
        }
        else if(updating)
        {
            icon.rectTransform.Rotate(new Vector3(0, 0, -Time.deltaTime * 360));
        }
    }

    public static void Disable()
    {
        if (Singleton != null)
        {
            Singleton.updating = false;
            Singleton.image.enabled = false;
            Singleton.icon.enabled = false;
            Singleton.Spacer.sizeDelta = new Vector3(Singleton.Spacer.sizeDelta.x, 0);
            Canvas.ForceUpdateCanvases();
            GameObject.FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
        }
    }
}
