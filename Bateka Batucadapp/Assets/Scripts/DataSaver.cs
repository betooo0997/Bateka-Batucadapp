using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public static void Save_User_Credentials()
    {
        PlayerPrefs.SetString("user_username", User.User_Info.Username);
        PlayerPrefs.SetString("user_pswd", User.User_Info.Psswd);
    }

    public static void Save_User_Data()
    {
        Save_User_Credentials();
        PlayerPrefs.SetString("user_role", User.User_Info.Role.ToString());
        PlayerPrefs.SetString("user_name", User.User_Info.Name);
        PlayerPrefs.SetString("user_surname", User.User_Info.Surname);
        PlayerPrefs.SetString("user_email", User.User_Info.Email);
        PlayerPrefs.SetString("user_tel", User.User_Info.Tel);
    }

    public static void Save_Database(string key, string database)
    {
        PlayerPrefs.SetString(key, database);
        PlayerPrefs.SetString(key + "_timestamp", System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
    }

    public static void Override_Database_entry(string key, string response, uint element_id)
    {
        if(!PlayerPrefs.HasKey(key))
        {
            Debug.LogError("Database not found!");
            return;
        }

        string datab = Utils.Split(response, "|")[1];
        string[] database = Utils.Split(Utils.Split(PlayerPrefs.GetString(key), "|")[1], "_DBEND_");

        for (int x = 0; x < database.Length; x++)
        {
            if (database[x].Contains("id$" + element_id.ToString() + "#"))
            {
                string data = Utils.Split(response, "|")[1].Replace("_DBEND_", "");
                database[x] = data;
                break;
            }
        }

        string stitched_database = "VERIFIED.|";

        foreach (string data_element in database)
            stitched_database += data_element + "_DBEND_";

        PlayerPrefs.SetString(key, stitched_database);
        PlayerPrefs.SetString(key + "_timestamp", System.DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"));
    }
}
