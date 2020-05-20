using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sound_Instance_Mono : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool Enabled, Repeated;

    public Sound_Instance_Mono Repeating_From;
    public List<Sound_Instance_Mono> Copies;

    [System.NonSerialized]
    public Sound_Type_Mono sound;

    [System.NonSerialized]
    public Sound_Data.Instance Instance;

    float fire_time, volume;
    string note;

    public float Fire_Time { get { return fire_time; } set { fire_time = value; if(Instance != null) Instance.Fire_Time = value; } }
    public float Volume { get { return volume; } set { Update_Volume(value); } }
    public string Note { get { return note; } set { note = value; if (Instance != null) Instance.Note = value; } }

    [System.NonSerialized]
    public Button Button;

    [System.NonSerialized]
    public Image Background;

    [System.NonSerialized]
    public Color Default_Color, Not_Repeated__Color, Repeated__Color;

    [System.NonSerialized]
    public EventHandler Toggling;

    [SerializeField]
    Image image = null;

    bool subscribed;

    private void Awake()
    {
        Background = GetComponent<Image>();
        Button = GetComponent<Button>();
        Copies = new List<Sound_Instance_Mono>();
        Default_Color = Background.color;
        Not_Repeated__Color = Default_Color;
        Repeated__Color = new Color(0.2f, 0.2f, 0.2f);
    }

    private void Start()
    {
        sound.Instances.Add(Fire_Time, this);
        Button.interactable = false;
    }

    private void OnDestroy()
    {
        sound.Instances.Remove(Fire_Time);
    }

    public void Load()
    {
        Fire_Time = Rhythm_Player.Round_To_Existing_Key_Floor(Fire_Time);
        Rhythm_Player.Singleton.Time_Events[Fire_Time] += On_Time;

        if (Instance != null)
            Volume = Instance.Volume;

        if (Enabled)
            Rhythm_Player.Singleton.Time_Events[Fire_Time] += sound.On_Time;           
    }

    public void Update_Volume(float value = -1)
    {
        if(value != -1)
            volume = value;

        if (Instance != null)
            Instance.Volume = volume;

        float height = GetComponent<RectTransform>().sizeDelta.y;
        height *= volume;

        RectTransform image_rect = image.GetComponent<RectTransform>();
        image_rect.sizeDelta = new Vector2(image_rect.sizeDelta.x, height);
    }


    public void Toggle_Enable()
    {
        if (Repeated || dragging)
            return;

        Update_Volume(1);
        Enabled = !Enabled;
        image.enabled = Enabled;
        Toggling?.Invoke(this, null);
        Rhythm_Player.Singleton.Reset_Events();

        List<Sound_Data.Instance> instances = Rhythm_Player.Singleton.Rhythms[0].Sounds_Data.Find(a => a.Type == sound.Sound_Type).Instances;

        if (!Enabled)
        {
            instances.Remove(Instance);
            Instance = null;
        }
        else if (!instances.Exists(a => a == Instance))
        {
            Instance = new Sound_Data.Instance() { Fire_Time = fire_time, Volume = volume, Note = note };
            instances.Add(Instance);
        }
    }

    public void Set_Enabled(bool enabled)
    {
        Enabled = enabled;
        image.enabled = enabled;
        Rhythm_Player.Singleton.Reset_Events();
    }

    public void Set_Repeated(Sound_Instance_Mono new_repeating_from)
    {
        if (subscribed)
        {
            Repeating_From.Toggling -= Copying_Toggle;
            subscribed = false;
        }

        if (new_repeating_from != null)
        {
            Repeated = true;
            Default_Color = Repeated__Color;

            Set_Enabled(new_repeating_from.Enabled);
            Volume = new_repeating_from.Volume;
            new_repeating_from.Copies.Add(this);
            new_repeating_from.Toggling += Copying_Toggle;
            subscribed = true;
        }
        else
        {
            Repeated = false;
            Default_Color = Not_Repeated__Color;

            if (Repeating_From != null && Repeating_From.Copies.Exists(a => a == this))
                Repeating_From.Copies.Remove(this);
        }

        Repeating_From = new_repeating_from;
        Background.color = Default_Color;
    }

    void On_Time(object sender, EventArgs e)
    {
        Background.color = new Color(0.8f, 0.8f, 0.8f);
        Invoke("Reset_Background", Rhythm_Player.Singleton.Step / Rhythm_Player.Singleton.Speed);
    }

    public void Reset_Background()
    {
        Background.color = Default_Color;
    }

    public void Copying_Toggle(object sender, EventArgs e)
    {
        Enabled = Repeating_From.Enabled;
        Update_Volume(Repeating_From.volume);
        image.enabled = Enabled;
    }

    private void DoForVolumeSetter<T>(Action<T> action) where T : IEventSystemHandler
    {
        foreach (var component in Volume_Setter.Singleton.GetComponents<Component>())
            if (component is T)
                action((T)(IEventSystemHandler)component);
    }

    private void DoForHorizontalScrollview<T>(Action<T> action) where T : IEventSystemHandler
    {
        foreach (var component in Rhythm_Player.Singleton.Sound_Instances_Root_Parent.GetComponents<Component>())
            if (component is T)
                action((T)(IEventSystemHandler)component);
    }

    bool route_to_horizontal;
    bool dragging;

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        route_to_horizontal = false;
        dragging = true;

        // Taking in account that the Calendar_Handler works as a horizontal scrollview
        if (Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y) || !Button.interactable)
        {
            route_to_horizontal = true;
            DoForHorizontalScrollview<IBeginDragHandler>((parent) => { parent.OnBeginDrag(eventData); });
        }
        else if (!Repeated)
        {
            Volume_Setter.Seting_Volume_Of = this;
            DoForVolumeSetter<IBeginDragHandler>((volume_setter) => { volume_setter.OnBeginDrag(eventData); });
        }
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (route_to_horizontal)
            DoForHorizontalScrollview<IDragHandler>((parent) => { parent.OnDrag(eventData); });
        else if (!Repeated)
            DoForVolumeSetter<IDragHandler>((volume_setter) => { volume_setter.OnDrag(eventData); });
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        if(route_to_horizontal)
            DoForHorizontalScrollview<IEndDragHandler>((parent) => { parent.OnEndDrag(eventData); });
        else if (!Repeated)
            DoForVolumeSetter<IEndDragHandler>((volume_setter) => { volume_setter.OnEndDrag(eventData); });

        Utils.InvokeNextFrame(() => { dragging = false; });
    }
}