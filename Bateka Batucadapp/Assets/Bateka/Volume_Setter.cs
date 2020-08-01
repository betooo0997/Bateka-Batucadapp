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
    RectTransform image_rect = null;

    [SerializeField]
    RectTransform canvas = null;

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
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        Seting_Volume_Of.Volume = volume;


        if(!Seting_Volume_Of.Enabled)
            Seting_Volume_Of.Set_Enabled(true);

        Seting_Volume_Of.Toggling?.Invoke(Seting_Volume_Of, null);
        Rhythm_Player.Singleton.Reset_Events();
    }
}
