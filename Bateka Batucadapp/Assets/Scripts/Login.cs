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
        string[] field_values = { "login", user.text, password .text };

        Http_Client.Send_Post(field_names, field_values, Handle_Login);
    }

    void Handle_Login(string response)
    {
        Debug.Log(response);

        if (response.Contains("LOGIN_SUCCESS."))
        {
            Load_Scene.LoadScene("Main");
        }
    }
}
