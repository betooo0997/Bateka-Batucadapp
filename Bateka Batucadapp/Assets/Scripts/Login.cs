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

    public static Login Singleton;

    void Awake()
    {
        Singleton = this;
    }

    public void Send_Login_Request()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_data", user.text, password.text };

        Http_Client.Send_Post(field_names, field_values, Handle_Login_Response);
    }

    void Handle_Login_Response(string response)
    {
        Parse_Login_Data(response, true);
    }

    public void Parse_Login_Data(string response, bool save = false)
    {
        string[] tokens = response.Split('|');
        string[] tokens_error = response.Split('*');

        if (tokens[0] == "VERIFIED.")
        {
            Load_Scene.Load_Scene_ST("Menu");
            User.Handle_User_Data(tokens[1]);

            if (save)
                DataSaver.Save_Database("user_database", response);

            else if (PlayerPrefs.HasKey("user_database_timestamp"))
                Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("user_database_timestamp"));
        }

        else if (tokens_error.Length > 1 && tokens_error[tokens_error.Length - 2] == "ERROR 500")
            Message.ShowMessage("Error 500 " + tokens_error[tokens_error.Length - 1]);

        else Message.ShowMessage("Error: usuario o contraseña incorrectos.");
    }
}
