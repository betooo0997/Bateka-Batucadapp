#pragma warning disable 0649

using System;
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

    Rhythm_Loop_Border end;

    float start;

    private void Awake()
    {
        loop = transform.parent.GetComponent<Rhythm_Loop>();
        rect_Transform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        foreach (Rhythm_Loop_Border border in transform.parent.GetComponentsInChildren<Rhythm_Loop_Border>())
            if (border.type == Border_Type.End)
                end = border;
    }

    public void Update_UI()
    {
        float height = loop.Sound.GetComponent<RectTransform>().rect.height;
        Vector2 size = rect_Transform.sizeDelta;

        switch (type)
        {
            case Border_Type.Start:
                rect_Transform.sizeDelta = new Vector2(rect_Transform.sizeDelta.x, height);
                transform.localPosition = new Vector3(size.x / 2, transform.localPosition.y);
                break;

            case Border_Type.End:
                rect_Transform.sizeDelta = new Vector2(rect_Transform.sizeDelta.x, height);
                transform.localPosition = new Vector3(loop.Get_Width_Core() - size.x / 2, transform.localPosition.y);
                break;

            case Border_Type.Up:
                rect_Transform.sizeDelta = new Vector2(loop.Get_Width_Core(), size.y);
                transform.localPosition = new Vector3(loop.Get_Width_Core() / 2, height / 2 - size.y / 2 + 0.5f);
                break;

            case Border_Type.Down:
                rect_Transform.sizeDelta = new Vector2(loop.Get_Width_Core(), size.y);
                transform.localPosition = new Vector3(loop.Get_Width_Core() / 2, -height / 2 + size.y / 2 - 0.5f);
                break;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if (type != Border_Type.End)
        {
            start = loop.transform.localPosition.x;
            return;
        }

        start = transform.localPosition.x;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (type != Border_Type.End)
        {
            loop.transform.localPosition += new Vector3(eventData.delta.x / transform.parent.lossyScale.x, 0);
            Debug.Log(loop.transform.localPosition);
            return;
        }

        transform.localPosition += new Vector3(eventData.delta.x / transform.lossyScale.x, 0);
        Debug.Log("Local X Position: " + transform.localPosition.x);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        float diff, length, time_diff;

        if (type != Border_Type.End)
        {
            diff = loop.transform.localPosition.x - start;

            Debug.Log("Diff: " + diff);

            length = loop.Data.Length;
            loop.Data.Start_Time = loop.Data.End_Time - length;
            time_diff = Rhythm_Player.Round_To_Existing_Key(Rhythm_Player.Position_To_Timer(diff, false));

            loop.transform.localPosition = new Vector3(start + Rhythm_Player.Timer_To_Position(time_diff, false), loop.transform.localPosition.y);
            Debug.Log("Time_Diff: " + time_diff);

            loop.Data.Start_Time += time_diff;
            loop.Data.End_Time += time_diff;

            loop.Update_Core();
            loop.Update_Periphery();
            return;
        }

        diff = transform.localPosition.x - start;

        Debug.Log("Diff: " + diff);

        time_diff = Rhythm_Player.Round_To_Existing_Key(Rhythm_Player.Position_To_Timer(diff, false));

        transform.localPosition = new Vector3(start + Rhythm_Player.Timer_To_Position(time_diff, false), transform.localPosition.y);
        Debug.Log("Time_Diff: " + time_diff);

        loop.Data.End_Time += time_diff;

        loop.Update_Core();
        loop.Update_Periphery();
        return;
    }
}
