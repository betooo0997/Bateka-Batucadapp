using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public void Logout()
    {
        User.User_Info = new User.User_Information { Username = "" };
        Load_Scene.Load_Scene_ST("Login", false);
    }

    public void Load_Menu_Scene(string scene)
    {
        Menu.Singleton.Load_Scene_Menu_Item(scene);
    }
}
