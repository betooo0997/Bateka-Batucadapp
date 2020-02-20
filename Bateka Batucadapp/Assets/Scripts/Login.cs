using System;
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

    public Button Login_Button;

    public static Login Singleton;

    private void Awake()
    {
        Singleton = this;
    }

    public void Send_Login_Request()
    {
        string[] field_names = { "REQUEST_TYPE", "username", "psswd" };
        string[] field_values = { "get_user_data", user.text, password.text };
        Http_Client.Send_Post(field_names, field_values, Handle_Login_Response, Handler_Type.none, false);
        Login_Button.interactable = false;
        Loading_Screen.Set_Active(true);
    }

    void Handle_Login_Response(string response, Handler_Type type)
    {
        Parse_Login_Data(response, true);
        Login_Button.interactable = true;
    }

    public static void Parse_Login_Data(string response, bool save = false)
    {
        string[] tokens = response.Split('|');
        string[] tokens_error = response.Split('*');

        if (tokens[0] == "VERIFIED.")
        {
            Load_Scene.Load_Scene_ST("Menu");
            Loading_Screen.Set_Active(true);

            User.Handle_User_Data(tokens[1]);

            if (save)
                DataSaver.Save_Database("user_database", response);

            else if (PlayerPrefs.HasKey("user_database_timestamp"))
                Message.ShowMessage("Fecha de datos: " + PlayerPrefs.GetString("user_database_timestamp"));
        }
        else
        {
            if (tokens_error.Length > 1 && tokens_error[tokens_error.Length - 2] == "ERROR 500")
                Message.ShowMessage("Error 500 " + tokens_error[tokens_error.Length - 1]);

            else Message.ShowMessage("Error: usuario o contraseña incorrectos.");

            Loading_Screen.Set_Active(false);
        }
    }
}
