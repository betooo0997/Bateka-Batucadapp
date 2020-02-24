using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class App_Updater : MonoBehaviour
{
    public const string APK_PATH = "https://kinderlandshop.es/wp-content/asambleapp/batekapp/batekapp.apk";
    public const float VERSION = 1.1f;

    public void Update_App()
    {
        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_version" };
        Http_Client.Send_Post(field_names, field_values, Handle_Version_Response);
        Loading_Screen.Set_Active(true);
    }

    void Handle_Version_Response(string response, Handler_Type none)
    {
        bool success = float.TryParse(Utils.Clear_Response(response), out float version);

        if (success)
        {
            if (response == VERSION.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." }))
                Message.ShowMessage("No hay ninguna actualización disponible en este momento.");
            else
            {
                Debug.Log(response + VERSION.ToString());
                Message.ShowMessage("¡Hay nuevas actualizaciones disponibles!");
                Message.ShowMessage("Abriendo enlace en tu navegador");
                Invoke("Open_In_Browser", 4);
            }
        }
        else
        {
            Message.ShowMessage("Respuesta del servidor no reconocida: " + response);
            Debug.LogError("Server response not recognized: " + response);
        }
    }

    void Open_In_Browser()
    {
        Application.OpenURL(APK_PATH);
    }
}
