using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Http_Client: MonoBehaviour
{
    public const string API_URL = "https://kinderlandshop.es/bateka-api-8420b25f4c1ad7ac906364ee943a7bef";
    static Http_Client Singleton;

    void Awake()
    {
        Singleton = this;
    }

    public static void Send_Post(string[] field_name, string[] field_value, Action<string> concludingMethod, bool add_user_credentials = true)
    {
        string data = "";

        for (int x = 0; x < field_name.Length; x++)
            data += field_name[x] + ": " + field_value[x] + "\n";

        Debug.Log("Sending HTTP Request:\n" + data);
        Singleton.StartCoroutine(Singleton.Send_Post_Coroutine(field_name, field_value, concludingMethod, add_user_credentials));
    }

    IEnumerator Send_Post_Coroutine(string[] field_name, string[] field_value, Action<string> concludingMethod, bool add_user_credentials)
    {
        WWWForm form = new WWWForm();
        form.AddField("API_USER", "USER");
        form.AddField("API_PASSWORD", "8420b25f4c1ad7ac906364ee943a7bef");

        if (add_user_credentials)
        {
            form.AddField("username", User.User_Info.Username);
            form.AddField("psswd", User.User_Info.Psswd);
        }

        for (int x = 0; x < field_name.Length; x++)
            form.AddField(field_name[x], field_value[x]);

        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, form))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Message.ShowMessage(www.error);
                Debug.LogWarning(www.error);
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");

                string response_header = sb.ToString();
                string response_content = www.downloadHandler.text;

                Debug.Log(response_content);
                concludingMethod(response_content);
            }
        }
    }

    public static void Download_Image(string url, Transform transform, Action<Transform, Texture2D> concludingMethod)
    {
        Debug.Log("Sending Image Request:\n" + url);
        Singleton.StartCoroutine(Singleton.Download_Image_Coroutine(url, transform, concludingMethod));
    }

    IEnumerator Download_Image_Coroutine(string url, Transform transform, Action<Transform, Texture2D> concludingMethod)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Message.ShowMessage(request.error);
            Debug.LogWarning(request.error);
        }
        else
            concludingMethod(transform, ((DownloadHandlerTexture)request.downloadHandler).texture);
    }
}