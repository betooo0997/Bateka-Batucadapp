﻿#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public static Login Singleton;

    public Button Login_Button;

    public GoogleAnalyticsV4 googleAnalytics;

    [SerializeField]
    InputField user, password = null;

    [SerializeField]
    Image background, login_reponse = null;

    [SerializeField]
    GameObject login_loading = null;

    bool response, success;

    float timer;

    string temp_psswd;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        Adapt_Background();
        googleAnalytics.StartSession();
    }

    void Update()
    {
        if(response)
        {
            if(login_reponse.color.a < 1)
                login_reponse.color += new Color(0, 0, 0, Time.deltaTime * 1.5f);
            else
            {
                timer += Time.deltaTime;

                if(timer > 1.5f)
                {
                    timer = 0;
                    login_loading.SetActive(false);
                    Login_Button.interactable = true;

                    if (success)
                        Utils.Load_Scene_ST("Menu");

                    response = false;
                    Invoke("Reset_Login_Button", 2);
                }
            }
        }
    }

    public void Set_Input_Fields()
    {
        user.text = User.User_Info.Username;
        string psswd = "";

        for (int x = 0; x < User.Psswd.Length; x++)
            psswd += "*";

        password.text = psswd;
        Login_Button.interactable = false;
    }

    void Reset_Login_Button()
    {
        login_reponse.color = new Color(1, 1, 1, 0);
    }

    public void Send_Login_Request()
    {
        User.Update_Data(user.text, password.text, true, true);
        Login_Button.interactable = false;
        login_loading.SetActive(true);
    }

    void Handle_Login_Response(string response, Handler_Type type)
    {
        User.Parse_User_Data(response, true);
    }

    public void On_Load_Success()
    {
        response = true;
        success = true;
        login_reponse.sprite = Helper.Singleton.Sprite_Login_Success;
        PlayerPrefs.SetString("version", App_Updater.VERSION.ToString());

        googleAnalytics.LogScreen("Login_S");
        googleAnalytics.LogEvent("Category_Example", "Event_Action", "Event_Label", 1);
        googleAnalytics.DispatchHits();
        Debug.Log("Logged");
    }

    public void On_Load_Failure()
    {
        response = true;
        success = false;
        login_reponse.sprite = Helper.Singleton.Sprite_Login_Failure;
    }

    void Adapt_Background()
    {
        float aspect_ratio_backgr = background.sprite.rect.width / background.sprite.rect.height;
        float aspect_ratio_screen = (float)Screen.width / Screen.height;

        RectTransform rect = background.GetComponent<RectTransform>();

        if (aspect_ratio_backgr < aspect_ratio_screen)
            rect.sizeDelta = new Vector2(Screen.width, Screen.width / aspect_ratio_backgr) / Utils.Canvas_Scale;
        else
            rect.sizeDelta = new Vector2(Screen.height * aspect_ratio_backgr, Screen.height ) / Utils.Canvas_Scale;
    }
}