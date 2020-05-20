#pragma warning disable 0649

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
    public static float Cell_Width;
    public List<Rhythm_Data> Rhythms;

    public float Timer;
    public float Timer_Key;
    public float Speed;
    public float Song_Length;
    public float Step { get; private set; }

    public Dictionary<float, EventHandler> Time_Events;
    public Dictionary<float, bool> Time_Events_Fired;

    public GameObject Sound_Instances_Root_Parent;

    [SerializeField]
    GameObject sound_instance_prefab, loop_prefab, separator_prefab, separator_dark_prefab, numeration_prefab;

    [SerializeField]
    Transform numeration_parent;

    [SerializeField]
    InputField rhythm_title, rhythm_speed, rhythm_length;

    [SerializeField]
    Dropdown time_signature;

    static float cells_per_second;

    static List<List<Image>> separators;
    static List<RectTransform> numerations;
    static Color color_separator_dark;
    static Color color_separator_light;

    float PPM { get { return Speed * 60; } set { Speed = value / 60; } }

    EventHandler[] event_handlers;
    int total_cells_count;
    bool playing;
    bool dragging;
    RectTransform content_rect_transform;


    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //

    void Awake()
    {
        Step = 0.125f;
        Singleton = this;
        content_rect_transform = transform.parent.GetComponent<RectTransform>();
        Rhythms = new List<Rhythm_Data>();
        separators = new List<List<Image>>();
        numerations = new List<RectTransform>();
        color_separator_dark = separator_dark_prefab.GetComponent<Image>().color;
        color_separator_light = separator_prefab.GetComponent<Image>().color;
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        total_cells_count = (int)Math.Round(Song_Length / Step, 0);
        event_handlers = new EventHandler[total_cells_count];
        cells_per_second = 1 / Step;
        Cell_Width = sound_instance_prefab.GetComponent<RectTransform>().sizeDelta.x;
        Load_Rhythm();

        foreach (Sound_Type_Mono sound in FindObjectsOfType<Sound_Type_Mono>())
        {
            List<Image> sep = new List<Image>();

            for (float x = 0; x < total_cells_count; x++)
            {
                Sound_Instance_Mono instance = Instantiate(sound_instance_prefab, sound.transform).GetComponent<Sound_Instance_Mono>();
                instance.Fire_Time = x * Step;
                instance.sound = sound;

                sep.Add(Instantiate(separator_prefab, sound.transform).GetComponent<Image>());
            }

            separators.Add(sep);
        }

        Utils.Update_UI = true;
    }

    void Update()
    {
        if (playing)
        {
            Timer += Time.deltaTime * Speed;

            if (Timer >= Song_Length)
            {
                Timer -= Song_Length;
                for (float x = 0; x < total_cells_count; x++)
                    Time_Events_Fired[x * Step] = false;
            }

            Timer_Key = Round_To_Existing_Key_Floor(Timer);

            if (!Time_Events_Fired[Timer_Key])
            {
                Time_Events_Fired[Timer_Key] = true;
                Time_Events[Timer_Key]?.Invoke(this, null);
            }
        }

        if (!dragging)
            transform.localPosition = new Vector3(Timer_To_Position(Timer), transform.localPosition.y);
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
        event_handlers = new EventHandler[total_cells_count];

        for (float x = 0; x < total_cells_count; x++)
        {
            Time_Events.Add(x * Step, event_handlers[(int)x]);
            Time_Events_Fired.Add(x * Step, false);
        }

        foreach (Sound_Instance_Mono instance in FindObjectsOfType<Sound_Instance_Mono>())
            instance.Load();
    }

    public void Change_PPM(string value)
    {
        PPM = float.Parse(value);
    }

    public void Change_Song_Length(string value_s)
    {
        float value = float.Parse(value_s);

        float diff = value - Song_Length;
        Song_Length = value;


        if (diff > 0)
        {
            foreach (Sound_Type_Mono sound in FindObjectsOfType<Sound_Type_Mono>())
            {
                for (int x = total_cells_count; x < total_cells_count + diff * cells_per_second; x++)
                {
                    Sound_Instance_Mono instance = Instantiate(sound_instance_prefab, sound.transform).GetComponent<Sound_Instance_Mono>();
                    instance.Fire_Time = x * Step;
                    instance.sound = sound;
                }
            }
        }
        else if (diff < 0)
        {
            foreach (Sound_Type_Mono sound in FindObjectsOfType<Sound_Type_Mono>())
            {
                for (int x = total_cells_count - 1; x > total_cells_count - 1 + diff * cells_per_second; x--)
                {
                    Destroy(sound.Instances[x * Step].gameObject);
                }
            }
        }

        total_cells_count = (int)Math.Round(Song_Length / Step, 0);
        Reset_Events();

        Utils.InvokeNextFrame(() => Utils.Reactivate(FindObjectOfType<Canvas>().gameObject));
        rhythm_length.text = Song_Length.ToString();
    }

    public void Increase_Song_Length()
    {
        Change_Song_Length((Song_Length + 1).ToString());
    }


    // ______________________________________
    //
    // 3. UTILS.
    // ______________________________________
    //



    public static float Round_To_Existing_Key_Floor(float number)
    {
        float rest = number % 1;
        rest = (float)Math.Floor(rest * cells_per_second) / cells_per_second;
        return (float)Math.Floor(number) + rest;
    }

    public static float Round_To_Existing_Key(float number)
    {
        float rest = number % 1;
        rest = (float)Math.Round(rest * cells_per_second, 0) / cells_per_second;

        float no_rest = (float)Math.Floor(number);

        if(number < 0)
            no_rest = (float)Math.Ceiling(number);

        return no_rest + rest;
    }

    public static float Position_To_Timer(float position, bool rect = true)
    {
        float rect_f = Singleton.content_rect_transform.rect.width / 2;

        if (!rect)
            rect_f = 0;

        return (position + rect_f) / Cell_Width / cells_per_second;
    }

    public static float Timer_To_Position(float timer, bool rect = true)
    {
        float rect_f = Singleton.content_rect_transform.rect.width / 2;

        if (!rect)
            rect_f = 0;

        return -rect_f + timer * Cell_Width * cells_per_second;
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
        transform.position = new Vector3(eventData.position.x, transform.position.y);

        float timer = Position_To_Timer(transform.localPosition.x);
        if (timer < 0)
            transform.localPosition = new Vector3(Timer_To_Position(0), transform.localPosition.y);
        else if (timer >= Song_Length)
            transform.localPosition = new Vector3(Timer_To_Position(Song_Length - 0.001f), transform.localPosition.y);
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Timer = Position_To_Timer(transform.localPosition.x);
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
        User.Psswd = "1234567891011121";

        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_rhythms" };
        Http_Client.Send_Post(field_names, field_values, Handle_Data_Response);
    }


    /// <summary>
    /// Parses JSON data to a Sound_Data class instance.
    /// </summary>
    Sound_Data Parse_Sound(Sound_Data.Sound_Type sound_type, string raw_sound_data)
    {
        SimpleJSON.JSONNode raw = SimpleJSON.JSON.Parse(raw_sound_data);
        Sound_Data new_sound = new Sound_Data { Type = sound_type };

        // Add Sound instances
        foreach (SimpleJSON.JSONNode element in raw[sound_type.ToString()][0])
        {
            string[] tokens = Utils.Split(element, '*');
            string notes = "";

            if (tokens.Length >= 3)
                notes = tokens[2];

            Sound_Data.Instance instance = new Sound_Data.Instance
            {
                Fire_Time = float.Parse(tokens[0], CultureInfo.InvariantCulture),
                Volume = float.Parse(tokens[1], CultureInfo.InvariantCulture),
                Note = notes
            };

            if (!new_sound.Instances.Exists(a => a.Fire_Time == instance.Fire_Time))
                new_sound.Instances.Add(instance);
        }

        // Add Loop instances
        foreach (SimpleJSON.JSONNode element in raw[sound_type.ToString()][1])
        {
            string[] tokens = Utils.Split(element, '*');

            Sound_Data.Loop loop = new Sound_Data.Loop
            {
                Start_Time = float.Parse(tokens[0], CultureInfo.InvariantCulture),
                End_Time = float.Parse(tokens[1], CultureInfo.InvariantCulture),
                Repetitions = uint.Parse(tokens[2])
            };

            if (!new_sound.Loops.Exists(a => a.Start_Time == loop.Start_Time || a.End_Time == loop.End_Time))
                new_sound.Loops.Add(loop);
        }

        return new_sound;
    }


    void Handle_Data_Response(string response, Handler_Type type)
    {
        string data = Utils.Split(response, '~')[1];

        foreach (string rhythm in Utils.Split(data, "%"))
        {
            Rhythm_Data new_rhythm = new Rhythm_Data();
            string[] rhythm_data = Utils.Split(rhythm, '#');

            new_rhythm.Id               = uint.Parse(rhythm_data[0]);
            new_rhythm.Name             = rhythm_data[1];
            new_rhythm.Description      = rhythm_data[2];
            new_rhythm.PPM              = uint.Parse(rhythm_data[3]);
            new_rhythm.Time_Signature   = (Rhythm_Data.Time_Signature_Type)Enum.Parse(typeof(Rhythm_Data.Time_Signature_Type), rhythm_data[4]);
            new_rhythm.Creation         = Utils.Get_DateTime(rhythm_data[5]);
            new_rhythm.Last_Update      = Utils.Get_DateTime(rhythm_data[6]);
            new_rhythm.Author_id        = uint.Parse(rhythm_data[7]);

            foreach (Sound_Data.Sound_Type sound_type in (Sound_Data.Sound_Type[])Enum.GetValues(typeof(Sound_Data.Sound_Type)))
            {
                if (sound_type == Sound_Data.Sound_Type.None)
                    continue;

                Sound_Data new_sound = Parse_Sound(sound_type, rhythm_data[8]);
                new_rhythm.Sounds_Data.Add(new_sound);

                if (!Sound_Type_Mono.Sounds.Exists(a => a.Sound_Type == new_sound.Type))
                    continue;

                Sound_Type_Mono sound_mono = Sound_Type_Mono.Sounds.Find(a => a.Sound_Type == new_sound.Type);

                foreach (Sound_Data.Instance instance in new_sound.Instances)
                {
                    sound_mono.Instances[instance.Fire_Time].Set_Enabled(true);
                    sound_mono.Instances[instance.Fire_Time].Instance = instance;
                }

                foreach (Sound_Data.Loop loop in new_sound.Loops)
                {
                    int sibling_index = sound_mono.Instances[loop.Start_Time].transform.GetSiblingIndex();

                    Rhythm_Loop rhythm_loop = Instantiate(loop_prefab, sound_mono.transform).GetComponent<Rhythm_Loop>();
                    rhythm_loop.Data = loop;
                    rhythm_loop.Sound = sound_mono;
                    rhythm_loop.Sound.Loops.Add(rhythm_loop);
                    rhythm_loop.Update_Core();
                    rhythm_loop.Update_Periphery();
                }
            }

            Rhythms.Add(new_rhythm);
        }

        rhythm_title.text = Rhythms[0].Name.ToString();
        PPM = Rhythms[0].PPM;
        rhythm_speed.text = PPM.ToString();
        rhythm_length.text = Song_Length.ToString();
        time_signature.value = (int)Rhythms[0].Time_Signature;
        time_signature.onValueChanged.AddListener((int value) => 
        {
            Rhythms[0].Time_Signature = (Rhythm_Data.Time_Signature_Type)value;
            Update_Separators();
        });

        for (int x = 1; x <= Song_Length; x++)
        {
            Text text = Instantiate(numeration_prefab, numeration_parent).GetComponentInChildren<Text>();
            text.text = x.ToString();
            RectTransform rect = text.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2((Cell_Width + 2) / Step * Rhythms[0].Get_Time_Modifier() - 2, rect.sizeDelta.y);
            numerations.Add(rect);
        }

        Update_Separators();

        Reset_Events();
        Utils.Update_UI = true;
    }

    void Update_Separators()
    {
        for (int x = 0; x < separators.Count; x++)
        {
            for (int y = 0; y < separators[x].Count; y++)
            {
                int beats_per_compass = Rhythms[0].Get_Beats_Per_Compass();

                if ((y + 1) % beats_per_compass == 0 && beats_per_compass == 4)
                    separators[x][y].color = color_separator_dark;
                else
                    separators[x][y].color = color_separator_light;
            }
        }

        float width = (Cell_Width + 2) / Step * Rhythms[0].Get_Time_Modifier() - 2;

        foreach (RectTransform rect in numerations)
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

        // TODO: Adjust Song_Length (Amount of compases) to changing time signatures and add / remove numerations accordingly.
    }

    public void Save_Rhythm()
    {
        User.User_Info.Username = "beto";
        User.Psswd = "1234567891011121";

        Rhythm_Data rhythm = Rhythms[0];

        string[] field_names = { "REQUEST_TYPE",
            "rhythm_id",
            "rhythm_name",
            "rhythm_details",
            "rhythm_ppm",
            "rhythm_date_update",
            "rhythm_date_creation",
            "rhythm_author_id",
            "rhythm_data" };

        string[] field_values = { "set_rhythms",
            rhythm.Id.ToString(),
            rhythm.Name,
            rhythm.Description,
            rhythm.PPM.ToString(),
            Utils.Get_String_SQL(rhythm.Last_Update),
            Utils.Get_String_SQL(rhythm.Creation),
            rhythm.Author_id.ToString(),
            rhythm.Get_Sounds_Json() };

        Http_Client.Send_Post(field_names, field_values, Handle_Save_Response);
    }

    void Handle_Save_Response(string response, Handler_Type type)
    {
        if (response.Contains("VERIFIED"))
            Message.ShowMessage("Ritmo guardado con éxito");
        else
            Message.ShowMessage("Error al guradar el ritmo");
    }
}
