using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Calendar_Handler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    private void Update()
    {
        if (!update)
            return;

        int change_value = -(current_month_element + 1);
        float target = (current_month_element) * month_element.sizeDelta.x;
        float movement = Time.deltaTime * (target - transform.localPosition.x) * 8 - change_value;

        transform.localPosition += new Vector3(movement, 0);

        if (Math.Abs(transform.localPosition.x - target) < 1)
        {
            transform.localPosition = new Vector3(-month_element.sizeDelta.x, transform.localPosition.y);
            update = false;

            if (change_value != 0)
            {
                month_to_show = month_to_show.AddMonths(change_value);

                months[0].Initialize(month_to_show.AddMonths(-1));
                months[1].Initialize(month_to_show);
                months[2].Initialize(month_to_show.AddMonths(1));
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


    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        if(!update)
            previous_mouse_pos = eventData.pressPosition;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!update)
        {
            transform.localPosition += new Vector3((eventData.position - previous_mouse_pos).x, 0);
            previous_mouse_pos = eventData.position;
        }
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        current_month_element = (int)Math.Round(transform.localPosition.x / month_element.sizeDelta.x, 0);
        update = true;
    }
}
