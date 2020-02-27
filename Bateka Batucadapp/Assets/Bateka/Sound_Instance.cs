using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Sound_Instance : MonoBehaviour
{
    [System.NonSerialized]
    public float Fire_Time;

    [System.NonSerialized]
    public Sound sound;

    [SerializeField]
    Image image;

    bool enabled;

    Image background;
    Color default_color;

    private void Awake()
    {
        background = GetComponent<Image>();
        default_color = background.color;
    }

    private void Start()
    {
        sound.Instances.Add(Fire_Time, this);
    }

    public void Load()
    {
        Fire_Time = Rhythm_Player.Round_To_Existing_Key(Fire_Time);
        Rhythm_Player.Singleton.Time_Events[Fire_Time] += On_Time;

        if (enabled)
            Rhythm_Player.Singleton.Time_Events[Fire_Time] += sound.On_Time;
    }

    public void Toggle_Enable()
    {
        enabled = !enabled;
        image.enabled = enabled;
        Rhythm_Player.Singleton.Reset_Events();
    }

    public void Set_Enabled(bool enabled)
    {
        this.enabled = enabled;
        image.enabled = enabled;
        Rhythm_Player.Singleton.Reset_Events();
    }


    void On_Time(object sender, EventArgs e)
    {
        background.color = new Color(0.8f, 0.8f, 0.8f);
        Invoke("Reset_Background", Rhythm_Player.Singleton.Step / Rhythm_Player.Singleton.Speed);
    }

    public void Reset_Background()
    {
        background.color = default_color;
    }

}