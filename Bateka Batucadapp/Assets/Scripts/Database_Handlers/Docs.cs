using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Docs : Database_Handler
{
    // ______________________________________
    //
    // 1. LOAD DATA.
    // ______________________________________
    //


    public static Doc Parse_Single_Data(string doc_data)
    {
        Doc doc = new Doc();

        foreach (string element in Utils.Split(doc_data, '#'))
        {
            string[] tokens = Utils.Split(element, '$');

            if (tokens.Length < 2) continue;
            switch (tokens[0])
            {
                case "id":
                    doc.Id = uint.Parse(tokens[1]);
                    break;

                case "title":
                    doc.Title = tokens[1];
                    break;

                case "subtitle":
                    doc.Subtitle = tokens[1];
                    break;

                case "details":
                    doc.Details = tokens[1];
                    break;

                case "creation_time":
                    doc.Creation_time = Utils.Get_DateTime(tokens[1]);
                    break;

                case "imgs":
                    foreach (string img in Utils.Split(tokens[1], '~'))
                        doc.Img_URLs.Add(img);
                    break;

                case "content_urls":
                    foreach (string url in Utils.Split(tokens[1], '~'))
                        doc.Content_URLs.Add(url);
                    break;
            }
        }

        return doc;
    }
}
