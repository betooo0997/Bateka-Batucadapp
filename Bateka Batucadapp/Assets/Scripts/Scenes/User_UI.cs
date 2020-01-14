using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class User_UI : MonoBehaviour
{
    [SerializeField]
    Text username;

    [SerializeField]
    Text real_name;

    [SerializeField]
    Text surname;

    [SerializeField]
    Text email;

    [SerializeField]
    Text tel;

    public static User.User_Information User_Shown;

    void Start()
    {
        username.text = User_Shown.Username;
        real_name.text = User_Shown.Name;
        surname.text = User_Shown.Surname;
        email.text = User_Shown.Email;
        tel.text = User_Shown.Tel;
    }
}
