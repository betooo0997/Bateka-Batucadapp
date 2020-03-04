using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rhythm_Loop_Border : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    enum Border_Type
    {
        Start,
        End,
        Up,
        Down
    }

    [SerializeField]
    Border_Type type;

    Rhythm_Loop loop;

    RectTransform rect_Transform;

    float start;

    bool late_update;

    private void Awake()
    {
        loop = transform.parent.GetComponent<Rhythm_Loop>();
        rect_Transform = GetComponent<RectTransform>();
    }

    public void Update_UI()
    {
        float height = loop.Sound.GetComponent<RectTransform>().rect.height;
        Debug.Log(height);
        Vector2 size = rect_Transform.sizeDelta;

        if (type <= Border_Type.End)
        {
            rect_Transform.sizeDelta = new Vector2(rect_Transform.sizeDelta.x, height);

            if (type == Border_Type.End)
                transform.localPosition = new Vector3(loop.Get_Width_Core() - size.x / 2, transform.localPosition.y);
            else
                transform.localPosition = new Vector3(size.x / 2, transform.localPosition.y);
        }
        else
        {
            rect_Transform.sizeDelta = new Vector2(loop.Get_Width_Core(), size.y);

            if(type == Border_Type.Up)
                transform.localPosition = new Vector3(loop.Get_Width_Core() / 2, height / 2 - size.y / 2 + 0.5f);
            else
                transform.localPosition = new Vector3(loop.Get_Width_Core() / 2, -height / 2 + size.y / 2 - 0.5f);
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (type >= Border_Type.Up)
            return;

        start = transform.localPosition.x;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (type >= Border_Type.Up)
            return;

        transform.localPosition = new Vector3((eventData.position.x - transform.parent.position.x) / transform.lossyScale.x, transform.localPosition.y);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (type >= Border_Type.Up)
            return;

        Vector2 size = rect_Transform.sizeDelta;

        float pos_x = Mathf.RoundToInt((transform.localPosition.x + size.x / 2) / 50) * 50;
        float time = Rhythm_Player.Position_To_Timer(transform.localPosition.x, false);
        time = Rhythm_Player.Round_To_Existing_Key(time);
        transform.localPosition = new Vector3(pos_x - size.x / 2, transform.localPosition.y);

        if (type == Border_Type.End)
        {
            loop.Data.End_Time = time - 0.125f;
        }

        loop.Update_Core();
        loop.Update_Periphery();
    }
}
