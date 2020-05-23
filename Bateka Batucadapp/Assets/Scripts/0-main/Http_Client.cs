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

    public enum Request_Type
    {
        GET,
        POST
    }

    void Awake()
    {
        if(Singleton == null)
            Singleton = this;
    }

    public static void Send_Post(string[] field_name, string[] field_value, Action<string, Handler_Type> concludingMethod, Handler_Type type = Handler_Type.none, bool add_user_credentials = true)
    {
        string data = "";

        for (int x = 0; x < field_name.Length; x++)
            data += field_name[x] + ": " + field_value[x] + "\n";

        Debug.Log("Sending HTTP Request:\n" + data);
        Singleton.StartCoroutine(Singleton.Send_Post_Coroutine(field_name, field_value, concludingMethod, add_user_credentials, type));
    }

    IEnumerator Send_Post_Coroutine(string[] field_name, string[] field_value, Action<string, Handler_Type> concludingMethod, bool add_user_credentials, Handler_Type type)
    {
        WWWForm form = new WWWForm();
        form.AddField("API_USER", "USER");
        form.AddField("API_PASSWORD", "8420b25f4c1ad7ac906364ee943a7bef");
        form.AddField("db_username", "dbu14967");
        form.AddField("db_password", "DrTgcePl06K#");

        if (add_user_credentials)
        {
            form.AddField("username", User.User_Info.Username);
            form.AddField("psswd", User.Psswd);
        }

        for (int x = 0; x < field_name.Length; x++)
            form.AddField(field_name[x], field_value[x]);

        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                string error = www.error;

                if (error == "Cannot resolve destination host")
                    error = "No estás conectad@ a internet.";

                Message.ShowMessage(error);
                Debug.LogWarning(error);
                Scroll_Updater.Disable();
                Login.Singleton.Login_Button.interactable = true;
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
                    sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");

                string response_header = sb.ToString();
                string response_content = www.downloadHandler.text;

                Debug.Log(response_content);
                concludingMethod(response_content, type);
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

    public static void Send_HTTP_Request(string url, Request_Type type, Dictionary<string, string> header, Action<string, string> concluding_method, string body = "")
    {
        switch(type)
        {
            case Request_Type.GET:
                Singleton.StartCoroutine(Singleton.Send_HTTP_GET_Request_Coroutine(url, header, concluding_method));
                break;

            case Request_Type.POST:
                if (body != "")
                    Singleton.StartCoroutine(Singleton.Send_HTTP_POST_Request_Coroutine(url, header, body, concluding_method));
                else
                    Debug.LogError("Body of HTTP POST Request cannot be empty!");
                break;
        }

        Debug.Log("Sending custom HTTP " + type.ToString() + " request");
    }

    IEnumerator Send_HTTP_GET_Request_Coroutine(string url, Dictionary<string,string> header, Action<string, string> concluding_method)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            foreach (KeyValuePair<string, string> entry in header)
                www.SetRequestHeader(entry.Key, entry.Value);

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
                On_Error(www);
            else
                On_Success(www, concluding_method);
        }
    }

    IEnumerator Send_HTTP_POST_Request_Coroutine(string url, Dictionary<string, string> header, string body, Action<string, string> concluding_method)
    {
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            foreach (KeyValuePair<string, string> entry in header)
                www.SetRequestHeader(entry.Key, entry.Value);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
                On_Error(www);
            else
                On_Success(www, concluding_method);
        }

        yield return null;
    }

    void On_Error(UnityWebRequest www)
    {
        string error = www.error;

        if (error == "Cannot resolve destination host")
            error = "No estás conectad@ a internet.";

        Message.ShowMessage(error);

        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
            sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");

        Debug.LogError("HTTP Response: " + error + "\nHeader:\n" + sb.ToString() + "\nContent:\n" + www.downloadHandler.text);
    }

    void On_Success(UnityWebRequest www, Action<string, string> concluding_method)
    {
        StringBuilder sb = new StringBuilder();

        foreach (KeyValuePair<string, string> dict in www.GetResponseHeaders())
            sb.Append(dict.Key).Append(": \t[").Append(dict.Value).Append("]\n");

        string response_header = sb.ToString();
        string response_content = www.downloadHandler.text;

        Debug.Log("HTTP Response:\nHeader:\n" + response_header + "\nContent:\n" + response_content);
        concluding_method(response_header, response_content);
    }
}