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

    public static object[] Bubble_Sort_DateTime(object[] list, DateTime[] date_time_list)
    {
        for (int x = 1; x < list.Length; x++)
        {
            bool switched = false;

            for (int y = 0; y < list.Length - x; y++)
            {
                if (Is_Sooner(date_time_list[y + 1], date_time_list[y]))
                {
                    object temp = list[y];
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
}
