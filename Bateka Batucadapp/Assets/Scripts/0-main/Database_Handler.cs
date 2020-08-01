using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Handler_Type
{
    none,
    news = Menu.Menu_item.News,
    polls = Menu.Menu_item.Polls,
    events = Menu.Menu_item.Events,
    docs = Menu.Menu_item.Docs
}

public abstract class Database_Handler : MonoBehaviour
{
    /// <summary>
    /// The only child class instance of Database_Handler that exists at the moment.
    /// </summary>
    public static Database_Handler Singleton;

    /// <summary>
    /// The only Data_struct instance that is selected at the moment.
    /// </summary>
    public static Data_struct Selected_Data;

    /// <summary>
    /// All existent Data_struct instances of all Database_Handlers children.
    /// </summary>
    static Dictionary<Type, List<Data_struct>> data_list;

    /// <summary>
    /// The Prefab containing a specific Data_UI child (*_summarized). To be spawned after parsing the corresponding database.
    /// </summary>
    [SerializeField]
    protected GameObject data_UI_prefab = null;

    /// <summary>
    /// The parent of the data_UI_prefab instance to be spwaned.
    /// </summary>
    [SerializeField]
    public Transform Data_UI_Parent = null;



    // ______________________________________
    //
    // 1. INITIALIZE.
    // ______________________________________
    //


    public static void Initialize_Dictionaries()
    {
        data_list = new Dictionary<Type, List<Data_struct>>();
    }



    // ______________________________________
    //
    // 2. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    protected virtual void Awake()
    {
        Singleton = this;
    }

    /// <summary>
    /// Spawns the UI elements after parsing the database.
    /// </summary>
    protected void Update()
    {
        if (Data_List_Get(GetType()).Count > 0)
            Spawn_UI_Elements();
    }

    void OnDestroy()
    {
        Singleton = null;
    }



    // ______________________________________
    //
    // 3. LOAD DATA.
    // ______________________________________
    //


    public static bool Is_Clean(string response)
    {
        return response.Contains("VERIFIED.");
    }

    /// <summary>
    /// Loads and parses a database from the device's cache, if existent.
    /// </summary>
    /// <param name="type">The child class type of Database_Handler that should load and parse its database.</param>
    public static void Load_Data_Cache(Handler_Type type)
    {
        string name = type.ToString() + "_database";

        if (PlayerPrefs.HasKey(name) && Is_Clean(name))
            Parse_Data(PlayerPrefs.GetString(type.ToString() + "_database"), false, type);
    }

    /// <summary>
    /// Loads a database from the server.
    /// </summary>
    /// <param name="type">The child class type of Database_Handler that should load and parse the server's database.</param>
    public static void Load_Data_Server(Handler_Type type)
    {
        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_" + type.ToString() };
        Http_Client.Send_Post(field_names, field_values, Handle_Data_Response, type);
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    /// <param name="response">The server's response.</param>
    /// <param name="type">The child class type of Database_Handler that is going to parse response.</param>
    static void Handle_Data_Response(string response, Handler_Type type)
    {
        Parse_Data(response, true, type);

        if (Calendar_Handler.Singleton != null)
            Calendar_Handler.Singleton.Initialize();
    }



    // ______________________________________
    //
    // 4. PARSE DATA.
    // ______________________________________
    //


    /// <summary>
    /// Parses a database gotten from the device's cache or from a server's response.
    /// </summary>
    /// <param name="data_to_parse">Data from cache or server to parse.</param>
    /// <param name="save">If data_to_parse should be saved on the device's cache.</param>
    /// <param name="handler_type">The child class type of Database_Handler that is going to parse data_to_parse.</param>
    static void Parse_Data(string data_to_parse, bool save, Handler_Type handler_type)
    {
        Func<string, Data_struct> Parse_Data_Single = null;
        Func<List<Data_struct>> Sort_List = null;
        Type type = null;

        switch (handler_type)
        {
            case Handler_Type.news:
                Parse_Data_Single = News.Parse_Single_Data;
                type = typeof(News);
                Sort_List = News.Sort_List;
                break;

            case Handler_Type.polls:
                Parse_Data_Single = Polls.Parse_Single_Data;
                type = typeof(Polls);
                Sort_List = Polls.Sort_List;
                break;

            case Handler_Type.events:
                Parse_Data_Single = Calendar_Events.Parse_Single_Data;
                type = typeof(Calendar_Events);
                Sort_List = Calendar_Events.Sort_List;
                break;

            case Handler_Type.docs:
                Parse_Data_Single = Docs.Parse_Single_Data;
                type = typeof(Docs);
                break;
        }

        List<Data_struct> data_list = new List<Data_struct>();

        if (save)
            Save_Database(handler_type.ToString() + "_database", data_to_parse);

        // Separate news database from initial server information. (E.g. "VERIFIED.|*databases*|")
        string data = Utils.Split(data_to_parse, '~')[1];

        // Separate each row to parse it individually. (E.g. "*row*%*row*")
        foreach (string element in Utils.Split(data, "%"))
            data_list.Add(Parse_Data_Single(element));

        Data_List_Set(type, data_list);

        if (Singleton != null)
            Singleton.enabled = true;

        Sort_List?.Invoke();
        Scroll_Updater.Disable();

        if (Menu.Active_Item == Menu.Menu_item.Home)
            foreach (Scrollable scrollable in FindObjectsOfType<Scrollable>())
                scrollable.Initialize();

        Post_Parse(handler_type);
    }

    static void Post_Parse(Handler_Type handler_type)
    {
        Type type = typeof(Docs);
        switch (handler_type)
        {
            case Handler_Type.events:
                type = typeof(Calendar_Events);
                foreach (Data_struct data in Data_List_Get(type))
                {
                    if (((Calendar_Event)data).Status == "" && (Singleton == null || Singleton.GetType() != type))
                    {
                        Helper.Menu_Icon = () => {
                            return Menu.Singleton.Button_Events.GetComponentInChildren<Image>();
                        };
                        Helper.Sprite_Red = () => {
                            return Menu.Singleton.Sprite_Events_Red;
                        };
                        Helper.Singleton.StartCoroutine("Update_Button");
                        break;
                    }
                }
                break;

            case Handler_Type.news:
                type = typeof(News);
                foreach (Data_struct data in Data_List_Get(type))
                {
                    if (!((News_Entry)data).Seen && (Singleton == null || Singleton.GetType() != type))
                    {
                        Helper.Menu_Icon = () => {
                            return Menu.Singleton.Button_News.GetComponentInChildren<Image>();
                        };
                        Helper.Sprite_Red = () => {
                            return Menu.Singleton.Sprite_News_Red;
                        };
                        Helper.Singleton.StartCoroutine("Update_Button");
                        break;
                    }
                }
                break;

            case Handler_Type.polls:
                type = typeof(Polls);
                foreach (Data_struct data in Data_List_Get(type))
                {
                    if (((Poll)data).Status == "" && (Singleton == null || Singleton.GetType() != type))
                    {
                        Helper.Menu_Icon = () => {
                            return Menu.Singleton.Button_Polls.GetComponentInChildren<Image>();
                        };
                        Helper.Sprite_Red = () => {
                            return Menu.Singleton.Sprite_Polls_Red;
                        };
                        Helper.Singleton.StartCoroutine("Update_Button");
                    }
                    break;
                }
            break;
        }

        Scroll_Updater.Initialized[type] = true;
    }

    public static void Save_Database(string key, string database)
    {
        PlayerPrefs.SetString(key, database);
        PlayerPrefs.SetString(key + "_timestamp", System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
    }


    // ______________________________________
    //
    // 5. DATA LIST.
    // ______________________________________
    //


    /// <summary>
    /// Gets the List of Data_struct elements of a specific child class type of Database_Handler.
    /// </summary>
    /// <param name="type">Child class type of Database_Handler</param>
    public static List<Data_struct> Data_List_Get(Type type)
    {
        if (data_list.ContainsKey(type))
            return data_list[type];
        else
            return new List<Data_struct>();
    }

    /// <summary>
    /// Sets the List of Data_struct elements of a specific child class type of Database_Handler.
    /// </summary>
    /// <param name="type">Child class type of Database_Handler.</param>
    /// <param name="list">The list that should be set to.</param>
    public static void Data_List_Set(Type type, List<Data_struct> list)
    {
        if (data_list.ContainsKey(type))
            data_list[type] = list;
        else
            data_list.Add(type, list);
    }

    /// <summary>
    /// Sets a specific element of the List of Data_struct elements of a specific child class type of Database_Handler.
    /// </summary>
    /// <param name="type">Child class type of Database_Handler.</param>
    /// <param name="idx">The index in the list.</param>
    /// <param name="element">The element that should be set to.</param>
    public static void Data_List_Set(Type type, int idx, Data_struct element)
    {
        if (data_list.ContainsKey(type))
            data_list[type][idx] = element;
        else
            Debug.LogError("data_list does not exist.");
    }



    // ______________________________________
    //
    // 6. DATA UI.
    // ______________________________________
    //


    /// <summary>
    /// Spawns all UI elements stored in its corresponding data_list.
    /// </summary>
    protected virtual void Spawn_UI_Elements()
    {
        foreach (Transform transform in Data_UI_Parent.GetComponentsInChildren<Transform>())
            if (transform.name == GetType().ToString() + "_element" || transform.name == GetType().ToString() + "_section")
                Destroy(transform.gameObject);

        foreach (Data_struct element in Data_List_Get(GetType()))
        {
            GameObject element_obj = Instantiate(data_UI_prefab, Data_UI_Parent);
            element_obj.name = GetType().ToString() + "_element";
            element_obj.GetComponent<Data_UI>().Set_Data(element);
        }

        enabled = false;
    }
}