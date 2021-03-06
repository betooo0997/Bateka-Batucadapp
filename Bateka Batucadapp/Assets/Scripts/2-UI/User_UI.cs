﻿#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User_UI : MonoBehaviour
{
    [SerializeField]
    Text username, real_name, surname, email, tel;

    public static User.User_Information User_Shown;

    void Start()
    {
        username.text = User_Shown.Username;
        real_name.text = User_Shown.Name;
        surname.text = User_Shown.Surname;

        if (User.User_Info.Role > 0)
        {
            email.text = User_Shown.Email;
            tel.text = User_Shown.Tel;
        }
        else
        {
            email.text = "-";
            tel.text = "-";
        }

        Utils.Update_UI = true;
    }
}
