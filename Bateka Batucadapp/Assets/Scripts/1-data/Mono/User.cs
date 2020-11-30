#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public enum User_Role
    {
        default_,
        moderator,
        admin
    }

    [System.Serializable]
    public struct User_Information
    {
        public uint Id;
        public string Username;
        public User_Role Role;
        public string Name;
        public string Surname;
        public string Email;
        public string Tel;
        public List<Vote_Data> Events_Data;
        public List<Vote_Data> Polls_Data;
        public List<uint> News_Data;
    }

    [System.Serializable]
    public class Vote_Data
    {
        public uint id;
        public int response;

        public static List<Vote_Data> Parse_Data(string data)
        {
            List<Vote_Data> result = new List<Vote_Data>();

            if (data == "empty")
                return result;

            string[] elements = Utils.Split(data, "|");

            foreach (string element in elements)
            {
                string[] pair = Utils.Split(element, "-");
                Vote_Data vote_data = new Vote_Data();
                vote_data.id = uint.Parse(pair[0]);
                vote_data.response = int.Parse(pair[1]);
                result.Add(vote_data);
            }

            return result;
        }
    }

    public static User_Information User_Info;
    public static List<User_Information> Users_Info;
    public static string Psswd;
    public static bool Initialized;

    [SerializeField]
    GameObject users_Prefab;

    [SerializeField]
    Transform user_parent;

    [SerializeField]
    Button user_own;

    static Action On_Success_temp;
    static Action OnFailure_temp;

    void Start()
    {
        user_own.onClick.RemoveAllListeners();
        user_own.onClick.AddListener(delegate { Show_Add_User_Info(User_Info.Username); });

        foreach (User_Information user_info in Users_Info)
        {
            GameObject new_user = Instantiate(users_Prefab, user_parent);
            new_user.name = "User " + user_info.Name;
            new_user.GetComponentInChildren<Text>().text = user_info.Username;
            new_user.GetComponent<Button>().onClick.AddListener(delegate { Show_Add_User_Info(user_info.Username); });
        }
    }

    public static void Handle_User_Data(string token)
    {
        User_Info = new User_Information { Username = "" };
        Users_Info = new List<User_Information>();

        string[] users = Utils.Split(token, '%');

        foreach (string user in users)
        {
            string[] data = Utils.Split(user, '#');

            for (int x = 0; x < data.Length; x++)
                data[x] = Encryption.Decrypt(data[x]);

            User_Information new_User = new User_Information();
            new_User.Id             = uint.Parse(data[0]);
            new_User.Username       = data[1];
            new_User.Name           = data[2];
            new_User.Surname        = data[3];
            Enum.TryParse(data[4], out new_User.Role);
            new_User.Tel            = data[5];
            new_User.Email          = data[6];
            new_User.Polls_Data     = Vote_Data.Parse_Data(data[7]);
            new_User.Events_Data    = Vote_Data.Parse_Data(data[8]);
            new_User.News_Data      = new List<uint>();

            if (!data[9].Contains("empty"))
                foreach (string element in Utils.Split(data[9], '|'))
                    new_User.News_Data.Add(uint.Parse(element));

            if (User_Info.Username == "") User_Info = new_User;
            else if (User_Info.Username != new_User.Username) Users_Info.Add(new_User);
        }
    }

    public void Show_Add_User_Info(string user_to_show)
    {
        User_UI.User_Shown = Get_User(user_to_show);
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Users_details);
    }

    public static User_Information Get_User(string username)
    {
        if (User_Info.Username == username)
            return User_Info;

        foreach (User_Information user_info in Users_Info)
            if (user_info.Username == username)
                return user_info;

        return new User_Information();
    }

    public static User_Information Get_User(uint id)
    {
        if (User_Info.Id == id)
            return User_Info;

        foreach (User_Information user_info in Users_Info)
            if (user_info.Id == id)
                return user_info;

        return new User_Information();
    }

    public static void Update_Data(string user = "", string psswd = "", bool load = true, bool save = false, Action On_Success = null, Action OnFailure = null)
    {
        Scroll_Updater.User_Loaded = 0;

        if (user == "")
        {
            user = User_Info.Username;
            psswd = Psswd;
        }

        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_user_data", user, psswd };
        Psswd = psswd;

        Http_Client.Send_Post(
            field_names,
            field_values,
            (string response, Handler_Type type) =>
            {
                Scroll_Updater.User_Loaded = 2;
                Parse_User_Data(response, load, save);
            },
            Handler_Type.none,
            false
            );

        On_Success_temp = On_Success;
        OnFailure_temp = OnFailure;
    }

    public static void Parse_User_Data(string response, bool load = true, bool save = false)
    {
        if (load)
        {
            On_Success_temp = Login.Singleton.On_Load_Success;
            OnFailure_temp = Login.Singleton.On_Load_Failure;
        }

        string[] tokens = Utils.Split(response, '~');
        string[] tokens_error = Utils.Split(response, '*');

        if (tokens[0] == "VERIFIED.")
        {
            Handle_User_Data(tokens[1]);

            if (save)
            {
                Debug.Log("Saving user database.");
                Database_Handler.Save_Database("user_database", response);
                PlayerPrefs.SetString("user_username", User_Info.Username);
                PlayerPrefs.SetString("user_psswd", Psswd);
                PlayerPrefs.SetString("version", Application.version.ToString());
            }

            Initialized = true;
            On_Success_temp?.Invoke();
        }
        else
        {
            if (!response.ToLower().Contains("wrong_credentials"))
                Message.ShowMessage("Error interno del servidor.");

            OnFailure_temp?.Invoke();
        }
    }
}

