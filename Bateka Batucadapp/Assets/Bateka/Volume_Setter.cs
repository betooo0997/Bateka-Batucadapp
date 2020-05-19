using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Volume_Setter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rect;
    public static Volume_Setter Singleton;
    public static Sound_Instance_Mono Seting_Volume_Of;

    [SerializeField]
    RectTransform image_rect;

    [SerializeField]
    RectTransform canvas;

    float volume;
    
    private void Awake()
    {
        Singleton = this;
        rect = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        gameObject.SetActive(true);
        transform.position = Seting_Volume_Of.transform.position;
        volume = Seting_Volume_Of.Volume;
        Debug.Log("V Drag Started");
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        float height = (eventData.position.y - image_rect.position.y) / canvas.localScale.x;

        if (height > rect.sizeDelta.y)
            height = rect.sizeDelta.y;

        if (height / rect.sizeDelta.y < 0.2f)
            height = rect.sizeDelta.y * 0.2f;

        image_rect.sizeDelta = new Vector2(image_rect.sizeDelta.x, height);

        volume = height / rect.sizeDelta.y;

        Debug.Log("V Draggin:" + volume);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        Debug.Log("V Drag Ended");
        Seting_Volume_Of.Volume = volume;

        foreach (Sound_Instance_Mono instance in Seting_Volume_Of.Copies)
            instance.Volume = volume;

        if(!Seting_Volume_Of.Enabled)
            Seting_Volume_Of.Set_Enabled(true);
    }
}
