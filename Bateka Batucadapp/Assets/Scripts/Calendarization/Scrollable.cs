using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scrollable : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform month_element;

    [SerializeField]
    LayoutElement horizontal_scrollview;

    Calendar_Month[] months;

    bool update, late_update = false;

    int current_month_element;

    DateTime month_to_show;

    Vector2 previous_mouse_pos;

    bool routeToParent = false;

    public Action<int> OnFinish;

    bool calendar;

    public static List<Calendar_Event> Shown_Events;
    Calendar_Events_UI_summarized[] shown_events_UI;

    int current_idx = 0;

    RectTransform canvas_rect;

    private void Awake()
    {
        calendar = GetComponent<Calendar_Handler>() != null;
        canvas_rect = FindObjectOfType<Canvas>().GetComponent<RectTransform>();

        if (!calendar)
            shown_events_UI = GetComponentsInChildren<Calendar_Events_UI_summarized>(true);
    }

    public void Initialize()
    {
        List<Data_struct> data = Database_Handler.Data_List_Get(typeof(Calendar_Events));
        Shown_Events = new List<Calendar_Event>();

        for (int x = 0; x < data.Count; x++)
        {
            if (x >= 5)
                break;
            Shown_Events.Add((Calendar_Event)data[x]);
        }

        if (!calendar && Shown_Events.Count > 0)
        {
            shown_events_UI[0].Set_Data(Shown_Events[Shown_Events.Count - 1]);
            shown_events_UI[0].gameObject.SetActive(true);

            for (int x = 1; x < shown_events_UI.Length; x++)
            {
                int idx = x - 1;

                if (idx >= Shown_Events.Count)
                    idx -= Shown_Events.Count;

                shown_events_UI[x].Set_Data(Shown_Events[idx]);
                shown_events_UI[x].gameObject.SetActive(true);
            }

            Idx_Handler.Singleton.Initialize(Shown_Events.Count);
        }
    }

    private void Update()
    {
        if (!update)
            return;

        int change_value = -(current_month_element + 1);

        float target = (current_month_element) * month_element.sizeDelta.x;

        bool signed = target - transform.localPosition.x < 0;

        if (!signed)
            transform.localPosition += new Vector3(Time.deltaTime * 750, 0);
        else
            transform.localPosition += new Vector3(Time.deltaTime * -750, 0);

        if (Math.Abs(transform.localPosition.x - target) < 1 || 
            (transform.localPosition.x - target) < 0 && signed ||
            (transform.localPosition.x - target) > 0 && !signed)
        {
            transform.localPosition = new Vector3(-month_element.sizeDelta.x, transform.localPosition.y);
            update = false;

            if (change_value != 0)
            {
                if (calendar)
                    Calendar_Handler.Singleton.OnFinish(change_value);
                else
                {
                    current_idx += change_value;

                    if (current_idx >= Shown_Events.Count)
                        current_idx = 0;

                    else if (current_idx < 0)
                        current_idx = Shown_Events.Count - 1;

                    Idx_Handler.Singleton.Update_idx(current_idx);

                    for (int x = 0; x < shown_events_UI.Length; x++)
                    {
                        int idx = x + current_idx - 1;

                        if (idx >= Shown_Events.Count)
                            idx -= Shown_Events.Count;

                        if (idx < 0)
                            idx += Shown_Events.Count;

                        shown_events_UI[x].Set_Data(Shown_Events[idx]);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!late_update)
            return;

        Canvas.ForceUpdateCanvases();
        foreach (VerticalLayoutGroup vLayout in FindObjectsOfType<VerticalLayoutGroup>())
            vLayout.SetLayoutVertical();
        horizontal_scrollview.minHeight = transform.parent.GetComponent<RectTransform>().sizeDelta.y;
        late_update = false;
    }

    // ______________________________________
    //
    // 4. DRAG HANDLING.
    // ______________________________________
    //


    void DoForParents<T>(Action<T> action) where T : IEventSystemHandler
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            foreach (var component in parent.GetComponents<Component>())
            {
                if (component is T)
                    action((T)(IEventSystemHandler)component);
            }
            parent = parent.parent;
        }
    }

    void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
    {
        DoForParents<IInitializePotentialDragHandler>((parent) => { parent.OnInitializePotentialDrag(eventData); });
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        // Taking in account that the Calendar_Handler works as a horizontal scrollview
        if (Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
        {
            routeToParent = true;
            DoForParents<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        }
        else
            routeToParent = false;

        if (!update)
            previous_mouse_pos = eventData.pressPosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!update && !routeToParent)
        {
            float canvas_scale = canvas_rect.localScale.x;
            transform.localPosition += new Vector3((eventData.position - previous_mouse_pos).x / canvas_scale, 0);
            previous_mouse_pos = eventData.position;
        }

        else if (routeToParent)
            DoForParents<IDragHandler>((parent) => { parent.OnDrag(eventData); });
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if (routeToParent)
            DoForParents<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else
        {
            current_month_element = (int)Math.Round(transform.localPosition.x / month_element.sizeDelta.x, 0);
            update = true;
        }
        routeToParent = false;
    }
}
