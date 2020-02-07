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
                    news_entry.Creation_time = Utils.Get_DateTime(tokens[1]);
                    break;

                case "imgs":
                    foreach (string img in Utils.Split(tokens[1], '~'))
                        news_entry.Img_URLs.Add(img);
                    break;
            }
        }

        return news_entry;
    }
}
