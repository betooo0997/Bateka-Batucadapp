using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scroll_Updater : MonoBehaviour
{
    public static Scroll_Updater Singleton;

    static Dictionary<Type, bool> initialized;

    bool updating;

    Image image;

    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    RectTransform Spacer;

    [SerializeField]
    Image icon;

    public static void Initialize()
    {
        initialized = new Dictionary<Type, bool>();
        initialized.Add(typeof(News), false);
        initialized.Add(typeof(Polls), false);
        initialized.Add(typeof(Calendar_Events), false);
    }

    private void Awake()
    {
        Singleton = this;
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (!initialized[Database_Handler.Singleton.GetType()] || (!updating && rectTransform.localPosition.y < -50 && Input.GetMouseButtonUp(0))) // TODO: Add touch!!
        {
            Database_Handler.Load_Data_Server((Handler_Type)Menu.Active_Item);

            updating = true;
            image.enabled = true;
            icon.enabled = true;
            initialized[Database_Handler.Singleton.GetType()] = true;
            Spacer.sizeDelta = new Vector3(Spacer.sizeDelta.x, 34);
            Canvas.ForceUpdateCanvases();
            FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
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
            FindObjectOfType<VerticalLayoutGroup>().SetLayoutVertical();
        }
    }
}
