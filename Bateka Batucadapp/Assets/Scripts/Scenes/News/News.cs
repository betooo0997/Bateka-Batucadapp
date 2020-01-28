using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News : Database_Handlers
{
    public static List<News_Entry> News_List;

    [SerializeField]
    GameObject news_entry_prefab;

    [SerializeField]
    Transform news_entry_parent;


    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    private void Update()
    {
        if (News_List != null && News_List.Count > 0)
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
        string[] field_values = { "get_news" };
        Http_Client.Send_Post(field_names, field_values, Handle_News_Response);
    }

    public static void Load_Data_Cache()
    {
        if (PlayerPrefs.HasKey("news_database"))
            Parse_News_Data(PlayerPrefs.GetString("news_database"));
    }

    /// <summary>
    /// Called on server response.
    /// </summary>
    static void Handle_News_Response(string response)
    {
        Parse_News_Data(response, true);
    }

    public static void Parse_News_Data(string response, bool save = false)
    {
        News_List = new List<News_Entry>();

        if (save)
            DataSaver.Save_Database("news_database", response);

        else if (PlayerPrefs.HasKey("news_database_timestamp"))
            Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("news_database_timestamp"));

        // Separate news database from initial server information. (E.g. "VERIFIED.|*news databases*|")
        string data = Utils.Split(response, '|')[1];

        // Separate each database to parse it individually. (E.g. "*database_1*_NDBEND_*database_2*")
        foreach (string news_entry in Utils.Split(data, "_NDBEND_"))
            News_List.Add(Parse_Single_News_Entry_Data(news_entry));

        if (Singleton_Exists(typeof(News)))
            Get_Singleton(typeof(News)).enabled = true;

        Scroll_Updater.Disable();
    }

    static News_Entry Parse_Single_News_Entry_Data(string news_entry_data)
    {
        if (news_entry_data.Contains("|"))
            news_entry_data = Utils.Split(news_entry_data, '|')[1];

        news_entry_data = news_entry_data.Replace("_NDBEND_", "");

        News_Entry news_entry = new News_Entry();

        foreach (string element in Utils.Split(news_entry_data, '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch (tokens[0])
            {
                case "id":
                    news_entry.Id = uint.Parse(tokens[1]);
                    break;

                case "title":
                    news_entry.Title = tokens[1];
                    break;

                case "details":
                    news_entry.Details = tokens[1];
                    break;

                case "creation_time":
                    news_entry.Creation_time = tokens[1];
                    break;

                case "imgs":
                    foreach (string img in Utils.Split(tokens[1], '~'))
                        news_entry.Img_URLs.Add(img);
                    break;
            }
        }

        return news_entry;
    }



    // ______________________________________
    //
    // 3. UPDATE UI.
    // ______________________________________
    //


    void Spawn_News_Entries_UI()
    {
        foreach (Transform transform in news_entry_parent.GetComponentsInChildren<Transform>())
            if(transform.name == "News_Entry")
                Destroy(transform.gameObject);

        foreach (News_Entry news_entry in News_List)
        {
            GameObject new_news_entry = Instantiate(news_entry_prefab, news_entry_parent);
            new_news_entry.name = "News_Entry";
            new_news_entry.GetComponent<News_summarized>().Set_Values(news_entry);
        }

        enabled = false;
    }
}
