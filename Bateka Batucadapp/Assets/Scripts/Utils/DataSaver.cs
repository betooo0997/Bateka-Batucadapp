using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public static void Save_Database(string key, string database)
    {
        PlayerPrefs.SetString(key, database);
        PlayerPrefs.SetString(key + "_timestamp", System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    public static void Override_Database_entry(string key, string response, uint element_id)
    {

    }
}
