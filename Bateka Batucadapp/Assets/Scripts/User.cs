using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public enum User_Role
    {
        default_,
        admin
    }

    public static string Username;
    public static string Psswd;
    public static User_Role Role;
    public static string Name;
    public static string Surname;
    public static string Email;
    public static string Tel;

    public static List<User> Users;

    private void Start()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_users", Username, Psswd };

        Http_Client.Send_Post(field_names, field_values, Handle_Get_Users_Response);
    }

    void Handle_Get_Users_Response(string response)
    {
        string[] tokens = response.Split('|');

        if (tokens[0] != "LOGIN_SUCCESS.")
        {
            Message.ShowMessage("Error: Credenciales erróneas. Contacta al administrador, aka Beto.");
            return;
        }

        string[] users = tokens[1].Split(new char[] { '%' }, System.StringSplitOptions.RemoveEmptyEntries);

        foreach (string user in users)
        {
            string[] pairs = user.Split(new char[] { '#' }, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string pair in pairs)
            {
                string[] elements = pair.Split(new char[] { '$' }, System.StringSplitOptions.RemoveEmptyEntries);

                switch (elements[0])
                {
                    case "username":
                        User.Username = elements[1];
                        break;

                    case "psswd":
                        User.Psswd = elements[1];
                        break;

                    case "role":
                        System.Enum.TryParse(elements[1], out User.Role);
                        break;

                    case "name":
                        User.Name = elements[1];
                        break;

                    case "surname":
                        User.Surname = elements[1];
                        break;

                    case "email":
                        User.Email = elements[1];
                        break;

                    case "tel":
                        User.Tel = elements[1];
                        break;
                }
            }
        }
    }
}

