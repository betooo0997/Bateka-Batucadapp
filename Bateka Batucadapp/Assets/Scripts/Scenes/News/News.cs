﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News : MonoBehaviour
{
    public static List<News_Entry> News_List;
    public List<News_Entry> Nnews_List;

    public static News_Entry Selected_News_Entry;


    // ______________________________________
    //
    // 1. MONOBEHAVIOUR LIFE CYCLE.
    // ______________________________________
    //


    void Start()
    {
        if (!PlayerPrefs.HasKey("news_database"))
        {
            string[] field_names = { "REQUEST_TYPE" };
            string[] field_values = { "get_news" };
            Http_Client.Send_Post(field_names, field_values, Handle_News_Response);
        }
        Nnews_List = News_List;
    }

    private void Update()
    {
        if (News_List != null && News_List.Count > 0)
        {
            Spawn_News_Entries_UI();
            enabled = false;
        }
    }


    /// <summary>
    /// Called on server response.
    /// </summary>
    void Handle_News_Response(string response)
    {
        Parse_News_Data(response, true);
        Nnews_List = News_List;
    }

    public static void Parse_News_Data(string response, bool save = false)
    {
        News_List = new List<News_Entry>();

        if (save)
            DataSaver.Save_Database("news_database", response);

        else if (PlayerPrefs.HasKey("news_database_timestamp"))
            Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("news_database_timestamp"));

        // Separate poll database from initial server information. (E.g. "VERIFIED.|*poll databases*|")
        string data = Utils.Split(response, '|')[1];

        // Separate each database to parse it individually. (E.g. "*database_1*_PDBEND_*database_2*")
        foreach (string news_entry in Utils.Split(data, "_NDBEND_"))
            News_List.Add(Parse_Single_News_Entry_Data(news_entry));
    }

    public static News_Entry Parse_Single_News_Entry_Data(string news_entry_data)
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
                    news_entry.Id = int.Parse(tokens[1]);
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

    void Spawn_News_Entries_UI()
    {
    }
}
