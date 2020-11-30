using System;
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

        for (int x = 0; x < data.Length; x++)
            data[x] = Encryption.Decrypt(data[x]);

        news_entry.Id               = uint.Parse(data[0]);
        news_entry.Title            = data[1];
        news_entry.Details          = data[2];
        news_entry.Creation_time    = Utils.Get_DateTime(data[3]);

        foreach (string element in Utils.Split(data[4], '|'))
            news_entry.Imgs.Add(element);

        news_entry.Author_Id        = uint.Parse(data[5]);
        news_entry.Privacy          = Utils.Parse_Privacy(data[6]);
        news_entry.Seen = User.User_Info.News_Data.Exists(a => a == news_entry.Id);

        return news_entry;
    }

    public static List<Data_struct> Sort_List()
    {
        List<Data_struct> Unsorted_List = News.Data_List_Get(typeof(News));
        DateTime[] date_Times = new DateTime[Unsorted_List.Count];

        for (int x = 0; x < date_Times.Length; x++)
            date_Times[x] = ((News_Entry)Unsorted_List[x]).Creation_time;

        List<Data_struct> Sorted_List = Utils.Bubble_Sort_DateTime(Unsorted_List, date_Times, true);
        return Sorted_List;
    }
}
