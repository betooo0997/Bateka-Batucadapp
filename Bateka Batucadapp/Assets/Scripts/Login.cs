﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public static Login Singleton;

    public Button Login_Button;

    [SerializeField]
    InputField user, password;

    [SerializeField]
    Image background, login_reponse;

    [SerializeField]
    GameObject login_loading;

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

                if(timer > 2)
                {
                    timer = 0;
                    login_loading.SetActive(false);
                    Login_Button.interactable = true;

                    if (success)
                        Load_Scene.Load_Scene_ST("Menu");

                    response = false;
                    Invoke("Reset_Login_Button", 2);
                }
            }
        }
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

        float canvas_scale = FindObjectOfType<Canvas>().GetComponent<RectTransform>().localScale.x;

        if (aspect_ratio_backgr < aspect_ratio_screen)
            rect.sizeDelta = new Vector2(Screen.width, Screen.width / aspect_ratio_backgr) / canvas_scale;
        else
            rect.sizeDelta = new Vector2(Screen.height * aspect_ratio_backgr, Screen.height ) / canvas_scale;
    }
}
