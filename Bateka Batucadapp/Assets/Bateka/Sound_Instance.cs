using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sound_Instance : MonoBehaviour
{
    public bool Repeated;

    public Sound_Instance Repeating_From;

    [System.NonSerialized]
    public Button Button;

    [System.NonSerialized]
    public float Fire_Time;

    [System.NonSerialized]
    public Sound sound;

    [SerializeField]
    Image image;

    public bool Enabled;

    [System.NonSerialized]
    public Image Background;

    [System.NonSerialized]
    public Color Default_Color;

    [System.NonSerialized]
    public Color Not_Repeated__Color;

    [System.NonSerialized]
    public EventHandler Toggling;

    bool subscribed;

    private void Awake()
    {
        Background = GetComponent<Image>();
        Button = GetComponent<Button>();
        Default_Color = Background.color;
        Not_Repeated__Color = Default_Color;
    }

    private void Start()
    {
        sound.Instances.Add(Fire_Time, this);
        Button.interactable = false;
    }

    public void Load()
    {
        Fire_Time = Rhythm_Player.Round_To_Existing_Key_Floor(Fire_Time);
        Rhythm_Player.Singleton.Time_Events[Fire_Time] += On_Time;

        if (Enabled)
            Rhythm_Player.Singleton.Time_Events[Fire_Time] += sound.On_Time;           
    }

    public void Toggle_Enable()
    {
        if (Repeated)
            return;

        Enabled = !Enabled;
        image.enabled = Enabled;
        Toggling?.Invoke(this, null);
        Rhythm_Player.Singleton.Reset_Events();
    }

    public void Set_Enabled(bool enabled)
    {
        Enabled = enabled;
        image.enabled = enabled;
        Rhythm_Player.Singleton.Reset_Events();
    }

    public void Set_Repeated(Sound_Instance new_repeating_from)
    {
        if (subscribed)
        {
            Repeating_From.Toggling -= Copying_Toggle;
            subscribed = false;
        }

        if (new_repeating_from != null)
        {
            Repeated = true;
            Default_Color = new Color(0, 0, 0.1f, 0.33f);

            Set_Enabled(new_repeating_from.Enabled);
            new_repeating_from.Toggling += Copying_Toggle;
            subscribed = true;
        }
        else
        {
            Repeated = false;
            Default_Color = Not_Repeated__Color;
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
        image.enabled = Enabled;
    }
}