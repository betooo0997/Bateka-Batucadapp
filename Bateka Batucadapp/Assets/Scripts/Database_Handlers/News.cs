using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News : Database_Handler
{
    // ______________________________________
    //
    // 1. LOAD DATA.
    // ______________________________________
    //


    public static News_Entry Parse_Single_Data(string news_entry_data)
    {
        News_Entry news_entry = new News_Entry();

        string[] data = Utils.Split(news_entry_data, '#');
        news_entry.Id               = uint.Parse(data[0]);
        news_entry.Title            = data[1];
        news_entry.Details          = data[2];
        news_entry.Creation_time    = Utils.Get_DateTime(data[3]);

        foreach (string element in Utils.Split(data[4], '|'))
            news_entry.Imgs.Add(element);

        news_entry.Author_Id        = int.Parse(data[5]);
        news_entry.Privacy          = Utils.Parse_Privacy(data[6]);

        return news_entry;
    }
}
