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

    string temp_psswd;

    private void Awake()
    {
        Singleton = this;
    }

    public void Send_Login_Request()
    {
        User.Update_Data(user.text, password.text);
        Login_Button.interactable = false;
        Loading_Screen.Set_Active(true);
    }

    void Handle_Login_Response(string response, Handler_Type type)
    {
        User.Parse_User_Data(response, true);
    }

    public void On_Load_Success()
    {
        Load_Scene.Load_Scene_ST("Menu");
        Loading_Screen.Set_Active(true);
        Login_Button.interactable = true;
    }

    public void On_Load_Failure()
    {
        Loading_Screen.Set_Active(false);
        Login_Button.interactable = true;
    }
}
