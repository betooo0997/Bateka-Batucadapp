using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public void Logout()
    {
        Load_Scene.Load_Scene_ST("Login", false);
    }
}
