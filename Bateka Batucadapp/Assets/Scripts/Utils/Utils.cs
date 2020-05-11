﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utils : MonoBehaviour
{
    public static Utils Singleton;

    static bool update_UI;
    public static bool Update_UI { get { return update_UI; } set { update_UI = value; if (value) updates = 2; } }
    static int updates;

    private void Awake()
    {
        Singleton = this;
    }

    private void LateUpdate()
    {
        if (updates > 0)
        {
            foreach (ContentSizeFitter fitter in FindObjectsOfType<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();
                fitter.SetLayoutHorizontal();
            }

            foreach (HorizontalOrVerticalLayoutGroup layout in FindObjectsOfType<HorizontalOrVerticalLayoutGroup>())
            {
                layout.SetLayoutVertical();
                layout.SetLayoutHorizontal();
            }

            foreach (ContentSizeFitter fitter in FindObjectsOfType<ContentSizeFitter>())
            {
                fitter.SetLayoutVertical();
                fitter.SetLayoutHorizontal();
            }

            Canvas.ForceUpdateCanvases();

            updates--;

            if (updates == 0)
                update_UI = false;
        }
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
        Debug.Log("Click");
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
        return date.ToString("dd.MM.yyyy | HH:mm") + "h";
    }

    public static string Get_String_SQL(DateTime date)
    {
        return date.ToString("yyyy-MM-dd HH:mm");
    }

    /// <summary>
    /// Parses a date string with format 'dd.mm.yyyy-hh:mm'.
    /// </summary>
    public static DateTime Get_DateTime(string date)
    {
        if (date.Length < 9)
            return new DateTime(9999, 12, 31);

        string[] data = Split(date, ' ');
        int[] day_data = Split_To_Int(data[0], '-');

        int[] hour_data;

        if (data.Length > 1)
            hour_data = Split_To_Int(data[1], ':');
        else
            hour_data = new int[2] { 0, 0 };

        return new DateTime(day_data[0], day_data[1], day_data[2], hour_data[0], hour_data[1], 0);
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

    public static void Unload_Time(int idx, float seconds)
    {
        Singleton.StartCoroutine(_Unload_Time(idx, seconds));
    }

    static IEnumerator _Unload_Time(int idx, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.UnloadSceneAsync(idx);
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
        if (response.Contains("~"))
            response = Split(response, '~')[1];

        return response;
    }

    static void Loop<T>(Transform parent, ref List<T> result)
    {
        for (int x = 0; x < parent.childCount; x++)
        {
            T component = parent.GetChild(x).GetComponent<T>();
            if (component != null)
                result.Add(component);

            Loop(parent.GetChild(x), ref result);
        }
    }

    static public List<T> GetComponentsInChildren_Order<T>(Transform parent)
    {
        List<T> result = new List<T>();

        Loop<T>(parent, ref result);

        return result;
    }

    public static Privacy Parse_Privacy(string data)
    {
        if (!Enum.TryParse(data.ToUpper()[0] + data.Substring(1), out Privacy result))
            Debug.LogError("Could not parse privacy setting '" + data.ToUpper()[0] + data.Substring(1) + "'");

        return result;
    }

    public static string Translate(string data)
    {
        switch (data)
        {
            case "rejection":
                return "Denegaciones";

            case "affirmation":
                return "Confirmaciones";

            default:
                return data;
        }
    }

    public static Color Darken_Color(Color color, float darkness)
    {
        if (color.r != 0)
            color = new Color(color.r - darkness * color.r, color.g, color.b);

        if (color.g != 0)
            color = new Color(color.r, color.g - darkness * color.g, color.b);

        if (color.b != 0)
            color = new Color(color.r, color.g, color.b - darkness * color.b);

        return color;
    }

    public static void Reactivate(GameObject target)
    {
        Loading_Screen.Set_Active(true);
        target.SetActive(false);

        InvokeNextFrame(() =>
        {
            target.gameObject.SetActive(true);
            Loading_Screen.Set_Active(false);
        });
    }

    public static string ToString(float data)
    {
        return data.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    public class EnumOrder<TEnum> where TEnum : struct
    {
        private static readonly TEnum[] Values;

        static EnumOrder()
        {
            var fields = Values.GetType().GetFields(BindingFlags.Static | BindingFlags.Public);
            Values = Array.ConvertAll(fields, x => (TEnum)x.GetValue(null));
        }

        public static int IndexOf(TEnum value)
        {
            return Array.IndexOf(Values, value);
        }
    }
}
