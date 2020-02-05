using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App_Updater : MonoBehaviour
{
    public static string APK_PATH = "https://kinderlandshop.es/wp-content/asambleapp/batekapp/batekapp.apk";

    bool checked_exists = true;

    public void Update_App()
    {
        if(checked_exists)
            Application.OpenURL(APK_PATH);
        else
        {
            // TODO: Check if newest version.
        }
    }
}
