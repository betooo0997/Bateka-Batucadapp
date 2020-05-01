using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Calendar_Handler : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static Calendar_Handler Singleton;

    [SerializeField]
    GameObject event_overview_prefab;

    [SerializeField]
    LayoutElement horizontal_scrollview;

    [SerializeField]
    RectTransform month_element;

    Calendar_Month[] months;

    bool update, late_update = false;

    int current_month_element;

    DateTime month_to_show;

    Vector2 previous_mouse_pos;

    bool routeToParent = false;



    // ______________________________________
    //
    // 1. INITIALIZE.
    // ______________________________________
    //


    public void Initialize()
    {
        months[0].Initialize(month_to_show.AddMonths(-1));
        months[1].Initialize(month_to_show);
        months[2].Initialize(month_to_show.AddMonths(1));

        foreach (Scrollable scrollable in FindObjectsOfType<Scrollable>())
            scrollable.Initialize();
    }



    // ______________________________________
    //
    // 2. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    void Awake()
    {
        Singleton = this;        
        months = GetComponentsInChildren<Calendar_Month>();
    }

    void Start()
    {
        // Get first of month.
        month_to_show = DateTime.Now.AddDays(-DateTime.Now.Day + 1); 
        Initialize();
    }

    public void OnFinish(int change_value)
    {
        month_to_show = month_to_show.AddMonths(change_value);

        months[0].Initialize(month_to_show.AddMonths(-1));
        months[1].Initialize(month_to_show);
        months[2].Initialize(month_to_show.AddMonths(1));
    }

    // ______________________________________
    //
    // 3. SHOW OVERVIEW.
    // ______________________________________
    //


    public void Show_Overview(Calendar_Day day)
    {
        foreach (Button button in GetComponentsInChildren<Button>())
            if(button.name.Contains("overview"))
                Destroy(button.gameObject);

        foreach (Calendar_Event calendar_event in day.Calendar_events)
        {
            Calendar_Overview overview = Instantiate(event_overview_prefab, months[1].transform).GetComponent<Calendar_Overview>();
            overview.SetValues(calendar_event);
        }

        late_update = true;
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
            transform.localPosition += new Vector3((eventData.position - previous_mouse_pos).x, 0);
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
