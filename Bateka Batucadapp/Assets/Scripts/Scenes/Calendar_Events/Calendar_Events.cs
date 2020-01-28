using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar_Events : MonoBehaviour
{
    public static List<Calendar_Event> Event_List;
    public List<Calendar_Event> Eevent_List;

    public static Calendar_Event Selected_Event;

    public static Calendar_Events Singleton;

    [SerializeField]
    GameObject events_prefab;

    [SerializeField]
    Transform events_parent;



    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    private void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        Eevent_List = Event_List;
    }

    private void Update()
    {
        if (Event_List != null && Event_List.Count > 0)
            Spawn_News_Entries_UI();
    }



    // ______________________________________
    //
    // 2. LOAD DATA.
    // ______________________________________
    //


    public static void Load_Data_Server()
    {
        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_events" };
        Http_Client.Send_Post(field_names, field_values, Handle_Events_Response);
    }

    public static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("events_database"))
            Parse_News_Data(PlayerPrefs.GetString("events_database"));
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    static void Handle_Events_Response(string response)
    {
        Parse_News_Data(response, true);

        if (Singleton != null)
            Singleton.Eevent_List = Event_List;
    }

    public static void Parse_News_Data(string response, bool save = false)
    {
        Event_List = new List<Calendar_Event>();

        if (save)
            DataSaver.Save_Database("events_database", response);

        else if (PlayerPrefs.HasKey("events_database_timestamp"))
            Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("events_database_timestamp"));

        // Separate news database from initial server information. (E.g. "VERIFIED.|*news databases*|")
        string data = Utils.Split(response, '|')[1];

        // Separate each database to parse it individually. (E.g. "*database_1*_NDBEND_*database_2*")
        foreach (string news_entry in Utils.Split(data, "_NDBEND_"))
            Event_List.Add(Parse_Single_News_Entry_Data(news_entry));

        if (Singleton != null)
            Singleton.enabled = true;

        Scroll_Updater.Disable();
    }

    static Calendar_Event Parse_Single_News_Entry_Data(string news_entry_data)
    {
        if (news_entry_data.Contains("|"))
            news_entry_data = Utils.Split(news_entry_data, '|')[1];

        news_entry_data = news_entry_data.Replace("_EDBEND_", "");

        Calendar_Event calendar_event = new Calendar_Event();

        foreach (string element in Utils.Split(news_entry_data, '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch (tokens[0])
            {
                case "id":
                    calendar_event.Id = uint.Parse(tokens[1]);
                    break;

                case "title":
                    calendar_event.Title = tokens[1];
                    break;

                case "details":
                    calendar_event.Details = tokens[1];
                    break;

                case "location":
                    calendar_event.Location = tokens[1];
                    break;

                case "date":
                    calendar_event.Date = Utils.Get_DateTime(tokens[1]);
                    break;

                case "confirm_deadline":
                    calendar_event.Confirm_Deadline = Utils.Get_DateTime(tokens[1]);
                    break;
            }
        }

        return calendar_event;
    }



    // ______________________________________
    //
    // 3. UPDATE UI.
    // ______________________________________
    //


    void Spawn_News_Entries_UI()
    {
        foreach (Transform transform in events_parent.GetComponentsInChildren<Transform>())
            if (transform.name == "Calendar_Event")
                Destroy(transform.gameObject);

        foreach (Calendar_Event calendar_event in Event_List)
        {
            GameObject new_news_entry = Instantiate(events_prefab, events_parent);
            new_news_entry.name = "Calendar_Event";
            new_news_entry.GetComponent<Calendar_Events_UI_summarized>().Set_Values(calendar_event);
        }

        enabled = false;
    }
}
