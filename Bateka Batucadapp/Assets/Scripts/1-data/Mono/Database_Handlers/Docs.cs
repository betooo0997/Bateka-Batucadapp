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

        string[] data       = Utils.Split(doc_data, '#');
        doc.Id              = uint.Parse(data[0]);
        doc.Title           = data[1];
        doc.Details         = data[2];
        doc.Creation_time   = Utils.Get_DateTime(data[3]);

        foreach (string url in Utils.Split(data[4], '~'))
            doc.Urls.Add(url);

        foreach (string element in Utils.Split(data[5], '|'))
            doc.Imgs.Add(element);

        doc.Author_Id       = uint.Parse(data[6]);
        doc.Privacy         = Utils.Parse_Privacy(data[7]);

        return doc;
    }
}
