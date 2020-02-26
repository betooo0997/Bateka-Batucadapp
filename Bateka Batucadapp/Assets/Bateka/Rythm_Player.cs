using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rythm_Player : MonoBehaviour
{
    bool playing;
    public float Timer;
    public float Speed;
    public float Song_Length;
    public static Rythm_Player Singleton;

    EventHandler[] event_handlers;
    public Dictionary<float, EventHandler> Time_Events;
    public Dictionary<float, bool> Time_Events_Fired;
    int events_count;

    [SerializeField]
    GameObject sound_instance_prefab;

    void Awake()
    {
        Singleton = this;
        events_count = (int)Math.Round(Song_Length / 0.125f, 0);
        event_handlers = new EventHandler[events_count];
    }

    private void Start()
    {
        Sound[] sounds = FindObjectsOfType<Sound>();

        foreach (Sound sound in sounds)
        {
            for (float x = 0; x < 8; x++)
            {
                Sound_Instance instance = Instantiate(sound_instance_prefab, sound.transform).GetComponent<Sound_Instance>();
                instance.Fire_Time = x * 0.125f;
                instance.sound = sound;
            }
        }

        Reload_Rythm();
    }

    void Update()
    {
        if (playing)
        {
            Timer += Time.deltaTime * Speed;
            if (Timer >= Song_Length)
                Reset_Timer();
            Check_Time_Events();
        }

        transform.localPosition = new Vector3(-203.5f + Timer * 50 * 8, 0);
    }

    public static float Round_To_Existing_Key(float number)
    {
        float rest = number % 1;
        rest = (float)Math.Floor(rest * 8) / 8;
        return (float)Math.Floor(number) + rest;
    }

    void Reset_Timer()
    {
        Timer -= Song_Length;

        for (float x = 0; x < events_count; x++)
            Time_Events_Fired[x * 0.125f] = false;
    }

    void Check_Time_Events()
    {
        float key = Round_To_Existing_Key(Timer);

        if (!Time_Events_Fired[key])
        {
            Time_Events_Fired[key] = true;
            
            if(Time_Events[key] != null)
                Time_Events[key].Invoke(this, null);
        }
    }

    void Reset_Subscribers()
    {
        Time_Events = new Dictionary<float, EventHandler>();
        Time_Events_Fired = new Dictionary<float, bool>();

        for (float x = 0; x < events_count; x++)
        {
            Time_Events.Add(x * 0.125f, event_handlers[(int)x]);
            Time_Events_Fired.Add(x * 0.125f, false);
        }
    }

    public void Toggle_Play()
    {
        playing = !playing;
    }

    public void Reload_Rythm()
    {
        Reset_Subscribers();

        foreach (Sound_Instance instance in FindObjectsOfType<Sound_Instance>())
            instance.Load();
    }
}
