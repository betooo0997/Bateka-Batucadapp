using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    [SerializeField]
    GameObject users_Prefab;

    [SerializeField]
    Transform user_parent;

    [SerializeField]
    Button user_own;

    public enum User_Role
    {
        default_,
        admin
    }

    [System.Serializable]
    public struct User_Information
    {
        public uint Id;
        public string Username;
        public string Psswd;
        public User_Role Role;
        public string Name;
        public string Surname;
        public string Email;
        public string Tel;
    }

    public static User_Information User_Info;
    public static List<User_Information> Users_Info;

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

        string[] users = token.Split(new char[] { '%' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string user in users)
        {
            string[] pairs = user.Split(new char[] { '#' }, System.StringSplitOptions.RemoveEmptyEntries);
            User_Information new_User = new User_Information();

            foreach (string pair in pairs)
            {
                string[] elements = pair.Split(new char[] { '$' }, System.StringSplitOptions.RemoveEmptyEntries);

                switch (elements[0])
                {
                    case "id":
                        new_User.Id = uint.Parse(elements[1]);
                        break;

                    case "username":
                        new_User.Username = elements[1];
                        break;

                    case "psswd":
                        new_User.Psswd = elements[1];
                        break;

                    case "role":
                        System.Enum.TryParse(elements[1], out new_User.Role);
                        break;

                    case "name":
                        new_User.Name = elements[1];
                        break;

                    case "surname":
                        new_User.Surname = elements[1];
                        break;

                    case "email":
                        new_User.Email = elements[1];
                        break;

                    case "tel":
                        new_User.Tel = elements[1];
                        break;
                }
            }

            if (User_Info.Username == "") User_Info = new_User;
            else if (User_Info.Username != new_User.Username) Users_Info.Add(new_User);
        }
    }

    public void Show_Add_User_Info(string user_to_show)
    {
        User_UI.User_Shown = Get_User(user_to_show);
        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Users_own);
    }

    public static User_Information Get_User(string username)
    {
        if (User_Info.Username == username)
            return User_Info;

        foreach(User_Information user_info in Users_Info)
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
}

