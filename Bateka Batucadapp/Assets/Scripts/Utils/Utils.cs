using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Utils Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public void Logout()
    {
        User.User_Info = new User.User_Information { Username = "" };
        Database_Handler.Initialize_Dictionaries();
        PlayerPrefs.DeleteAll();
        Load_Scene.Load_Scene_ST("Login", false);
    }

    public void Load_Menu_Scene(string scene)
    {
        Menu.Singleton.Load_Scene_Menu_Item(scene);
    }

    public static string[] Split(string to_split, string[] splitter)
    {
        return to_split.Split(splitter, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] Split(string to_split, string splitter)
    {
        return to_split.Split(new string[] { splitter }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static string[] Split(string to_split, char splitter)
    {
        return to_split.Split(new char[] { splitter }, System.StringSplitOptions.RemoveEmptyEntries);
    }

    public static int[] Split_To_Int(string to_split, char splitter)
    {
        string[] splitted = to_split.Split(new char[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
        int[] result = new int[splitted.Length];

        for (int x = 0; x < result.Length; x++)
            int.TryParse(splitted[x], out result[x]);

        return result;
    }

    public static string Get_String(DateTime date)
    {
        return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString() 
            + " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString();
    }

    /// <summary>
    /// Parses a date string with format 'dd.mm.yyyy|hh.mm'.
    /// </summary>
    public static DateTime Get_DateTime(string date)
    {
        if (date.Length < 9)
            return new DateTime(9999, 12, 31);

        string[] data = Split(date, '-');
        int[] day_data = Split_To_Int(data[0], '.');
        int[] hour_data = Split_To_Int(data[1], '.');

        return new DateTime(day_data[2], day_data[1], day_data[0], hour_data[0], hour_data[1], 0);
    }

    public static bool Is_Sooner(DateTime a, DateTime b)
    {
        int result = DateTime.Compare(a, b);

        if (result < 0)
            return true;

        return false;
    }

    public static List<Data_struct> Bubble_Sort_DateTime(List<Data_struct> list, DateTime[] date_time_list)
    {
        for (int x = 1; x < list.Count; x++)
        {
            bool switched = false;

            for (int y = 0; y < list.Count - x; y++)
            {
                if (Is_Sooner(date_time_list[y + 1], date_time_list[y]))
                {
                    Data_struct temp = list[y];
                    DateTime dt_temp = date_time_list[y];

                    list[y] = list[y + 1];
                    date_time_list[y] = date_time_list[y + 1];
                    list[y + 1] = temp;
                    date_time_list[y + 1] = dt_temp;
                    switched = true;
                }
            }

            if (!switched)
                break;
        }
        return list;
    }

    public delegate void Function();

    public static void InvokeNextFrame(Function function)
    {
        try
        {
            Singleton.StartCoroutine(_InvokeNextFrame(function));
        }
        catch
        {
            Debug.Log("Trying to invoke " + function.ToString() + " but it doesn't seem to exist");
        }
    }

    static IEnumerator _InvokeNextFrame(Function function)
    {
        yield return null;
        function();
    }

    /// <summary>
    /// Get list of users from a data string.
    /// </summary>
    public static List<User.User_Information> Get_Voters(string data)
    {
        string[] user_ids = Utils.Split(data, ',');
        List<User.User_Information> vote_list = new List<User.User_Information>();

        foreach (string user_id in user_ids)
        {
            User.User_Information voter = User.Get_User(uint.Parse(user_id));
            if (voter.Id != 0)
                vote_list.Add(voter);
        }

        return vote_list;
    }

    public static string Clear_Response(string response)
    {
        if (response.Contains("|"))
            response = Split(response, '|')[1];

        response = response.Replace("_PDBEND_", "");

        return response;
    }
}
