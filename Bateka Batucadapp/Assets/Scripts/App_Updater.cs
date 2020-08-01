#pragma warning disable 0649

using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class App_Updater : MonoBehaviour
{
    public const string APK_PATH = "https://kinderlandshop.es/wp-content/asambleapp/batekapp/tabalapp";

    [SerializeField]
    Text version;

    void Start()
    {
        version.text = "Tabalapp v" + Application.version.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }

    public void Update_App()
    {
        string[] field_names = { "REQUEST_TYPE" };
        string[] field_values = { "get_version" };
        Http_Client.Send_Post(field_names, field_values, Handle_Version_Response);
        Loading_Screen.Set_Active(true);
    }

    void Handle_Version_Response(string response, Handler_Type none)
    {
        response = Utils.Clear_Response(response);
        bool success = float.TryParse(response, out float version);

        if (success)
        {
            if (response == Application.version.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." }))
                Message.ShowMessage("No hay ninguna actualización disponible en este momento.");
            else
            {
                Debug.Log(response + Application.version.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." }));
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
        Application.OpenURL(APK_PATH + Application.version + ".apk");
    }
}
