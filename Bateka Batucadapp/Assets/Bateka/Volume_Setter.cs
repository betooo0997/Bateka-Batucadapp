using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Volume_Setter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static Volume_Setter Singleton;
    public static Sound_Instance Seting_Volume_Of;

    private void Awake()
    {
        Singleton = this;
        gameObject.SetActive(false);
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        gameObject.SetActive(true);
        transform.position = Seting_Volume_Of.transform.position;
        Debug.Log("V Drag Started");
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("V Draggin");
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        gameObject.SetActive(false);
        Debug.Log("V Drag Ended");
    }
}
