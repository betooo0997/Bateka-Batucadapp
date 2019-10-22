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

    void Start()
    {
        username.text = User.Username;
        real_name.text = User.Name;
        surname.text = User.Surname;
        email.text = User.Email;
        tel.text = User.Tel;
    }
}
