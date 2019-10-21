using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HttpClient: MonoBehaviour
{
    public const string API_URL = "https://kinderlandshop.es/bateka-api-8420b25f4c1ad7ac906364ee943a7bef";

    void Start()
    {
        StartCoroutine(SendPostCoroutine());
    }

    /// <summary>
    /// Send a HTTP Request to the client database server.
    /// </summary>

    IEnumerator SendPostCoroutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("API_USER", "USER");
        form.AddField("API_PASSWORD", "8420b25f4c1ad7ac906364ee943a7bef");

        form.AddField("REQUEST_TYPE", "login");
        form.AddField("username", "beto");
        form.AddField("psswd", "test123");

        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, form))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("POST successful!");
                StringBuilder sb = new StringBuilder();
                foreach (System.Collections.Generic.KeyValuePair<string, string> dict in www.GetResponseHeaders())
                {
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");
                }

                // Print Headers
                Debug.Log(sb.ToString());

                // Print Body
                Debug.Log(www.downloadHandler.text);
            }
        }
    }
}
