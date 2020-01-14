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

    public void Send_Login_Request()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_data", user.text, password.text };

        Http_Client.Send_Post(field_names, field_values, Handle_Login_Response);
    }

    void Handle_Login_Response(string response)
    {
        string[] tokens = response.Split('|');
        string[] tokens_error = response.Split('*');

        if (tokens[0] == "VERIFIED.")
        {
            Load_Scene.Load_Scene_ST("Menu");
            User.Handle_User_Data(tokens[1]);
        }

        else if (tokens_error.Length > 1 && tokens_error[tokens_error.Length - 2] == "ERROR 500")
            Message.ShowMessage("Error 500 " + tokens_error[tokens_error.Length - 1]);

        else Message.ShowMessage("Error: usuario o contraseña incorrectos.");
    }
}
