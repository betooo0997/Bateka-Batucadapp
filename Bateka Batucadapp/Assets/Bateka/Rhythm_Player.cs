using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Rhythm_Player : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static Rhythm_Player Singleton;
    public List<Rhythm> Rhythms;

    public float Timer;
    public float Speed;
    public float Song_Length;
    public float Step { get; private set; }

    public Dictionary<float, EventHandler> Time_Events;
    public Dictionary<float, bool> Time_Events_Fired;

    [SerializeField]
    GameObject sound_instance_prefab;

    EventHandler[] event_handlers;
    int events_count;
    bool playing;
    bool dragging;


    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    void Awake()
    {
        Step = 0.125f;
        Singleton = this;
        Rhythms = new List<Rhythm>();
        events_count = (int)Math.Round(Song_Length / Step, 0);
        event_handlers = new EventHandler[events_count];
    }

    private void Start()
    {
        Loading_Screen.Set_Active(true, 1);

        Sound[] sounds = FindObjectsOfType<Sound>();

        foreach (Sound sound in sounds)
        {
            for (float x = 0; x < 8; x++)
            {
                Sound_Instance instance = Instantiate(sound_instance_prefab, sound.transform).GetComponent<Sound_Instance>();
                instance.Fire_Time = x * Step;
                instance.sound = sound;
            }
        }

        Load_Rhythm();
    }

    void Update()
    {
        if (playing)
        {
            Timer += Time.deltaTime * Speed;

            if (Timer >= Song_Length)
            {
                Timer -= Song_Length;
                for (float x = 0; x < events_count; x++)
                    Time_Events_Fired[x * Step] = false;
            }

            float key = Round_To_Existing_Key(Timer);

            if (!Time_Events_Fired[key])
            {
                Time_Events_Fired[key] = true;
                Time_Events[key]?.Invoke(this, null);
            }
        }

        if(!dragging)
            transform.localPosition = new Vector3(-203.5f + Timer * 50 * 8, transform.localPosition.y);
    }



    // ______________________________________
    //
    // 2. HANDLING LOADED RHYTHM.
    // ______________________________________
    //


    public void Toggle_Play()
    {
        playing = !playing;
    }

    public void Reset_Events()
    {
        Time_Events = new Dictionary<float, EventHandler>();
        Time_Events_Fired = new Dictionary<float, bool>();

        for (float x = 0; x < events_count; x++)
        {
            Time_Events.Add(x * Step, event_handlers[(int)x]);
            Time_Events_Fired.Add(x * Step, false);
        }

        foreach (Sound_Instance instance in FindObjectsOfType<Sound_Instance>())
            instance.Load();
    }


    // ______________________________________
    //
    // 3. UTILS.
    // ______________________________________
    //



    public static float Round_To_Existing_Key(float number)
    {
        float rest = number % 1;
        rest = (float)Math.Floor(rest * 8) / 8;
        return (float)Math.Floor(number) + rest;
    }



    // ______________________________________
    //
    // 4. DRAG HANDLING.
    // ______________________________________
    //


    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position += new Vector3(eventData.delta.x, 0);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Timer = (transform.localPosition.x + 203.5f) / 50 / 8;
        transform.localPosition = new Vector3(-203.5f + Timer * 50 * 8, transform.localPosition.y);
        Reset_Events();

        dragging = false;
    }



    // ______________________________________
    //
    // 5. RHYTHM LOADING / SAVING.
    // ______________________________________
    //


    public void Load_Rhythm()
    {
        User.User_Info.Username = "beto";
        User.User_Info.Psswd = "0001";
        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_rhythms" };
        Http_Client.Send_Post(field_names, field_values, Handle_Data_Response);
    }

    void Handle_Data_Response(string response, Handler_Type type)
    {
        string data = Utils.Split(response, '|')[1];

        foreach (string rhythm in Utils.Split(data, "_DBEND_"))
        {
            Rhythm new_rhythm = new Rhythm();

            foreach (string element in Utils.Split(rhythm, '#'))
            {
                string[] tokens = Utils.Split(element, '$');

                if (tokens.Length < 2) continue;
                switch (tokens[0])
                {
                    case "id":
                        new_rhythm.Id = uint.Parse(tokens[1]);
                        break;

                    case "last_modification_date":
                        new_rhythm.Last_Modification_Date = Utils.Get_DateTime(tokens[1]);
                        break;

                    case "sound":
                        Rhythm.Sound sound = new Rhythm.Sound();
                        string[] sound_elements = Utils.Split(tokens[1], '~');

                        string sound_type = sound_elements[0];

                        if (!Enum.TryParse(sound_type.ToUpper()[0] + sound_type.Substring(1), out sound.Type))
                            Debug.LogError("Sound Type could not be parsed.");

                        Sound sound_mono = Sound.Sounds.Find(a => a.Sound_Type == sound.Type);

                        for (int x = 1; x < sound_elements.Length; x++)
                        {
                            string[] instance_parts = Utils.Split(sound_elements[x], 'd');

                            Rhythm.Sound.Instance sound_instance = new Rhythm.Sound.Instance
                            {
                                Fire_Time = Round_To_Existing_Key(float.Parse(instance_parts[0], CultureInfo.InvariantCulture)),
                                Volume = Round_To_Existing_Key(float.Parse(instance_parts[1], CultureInfo.InvariantCulture)),
                                Note = instance_parts[2]
                            };

                            sound.Instances.Add(sound_instance);
                            sound_mono.Instances[sound_instance.Fire_Time].Toggle_Enable(); ;
                        }

                        new_rhythm.Sounds.Add(sound);
                        break;
                }
            }

            Rhythms.Add(new_rhythm);
        }

        Reset_Events();
        GameObject canvas = FindObjectOfType<Canvas>().gameObject;
        canvas.SetActive(false);

        Utils.InvokeNextFrame(() => 
        {
            canvas.SetActive(true);
            Loading_Screen.Set_Active(false);
        });
           }

    public void Save_Rhythm()
    {

    }
}
