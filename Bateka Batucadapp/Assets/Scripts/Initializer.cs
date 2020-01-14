using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("user_database"))
            Login.Singleton.Parse_Login_Data(PlayerPrefs.GetString("user_database"));
    }
}
