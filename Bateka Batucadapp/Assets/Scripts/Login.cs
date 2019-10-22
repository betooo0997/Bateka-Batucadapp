using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    InputField user;

    [SerializeField]
    InputField password;

    public void Bateka_Login()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "login", user.text, password.text };

        Http_Client.Send_Post(field_names, field_values, Handle_Login_Response);
    }

    void Handle_Login_Response(string response)
    {
        Debug.Log(response);
        string[] tokens = response.Split('|');

        if (tokens[0] == "LOGIN_SUCCESS.")
        {
            Load_Scene.Load_Scene_ST("Menu");
            Parse_Login_Response(tokens[1]);
        }
        else Message.ShowMessage("Error: usuario o contraseña incorrectos.");
    }

    void Parse_Login_Response(string token)
    {
        string[] pairs = token.Split(new char[] { '#' }, System.StringSplitOptions.RemoveEmptyEntries);

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
