using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSaver : MonoBehaviour
{
    public static void Save_User_Credentials()
    {
        PlayerPrefs.SetString("user_username", User.User_Info.Username);
        PlayerPrefs.SetString("user_pswd", User.Psswd);
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

    }
}
