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
    Image icon;

    Image image;

    bool updating;

    private void Awake()
    {
        Singleton = this;
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!updating && rectTransform.localPosition.y < -50 && Input.GetMouseButtonUp(0)) // TODO: Add touch!!
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
        }
    }
}
