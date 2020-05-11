using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News_detail : News_UI
{
    public Text Detail;

    [SerializeField]
    GameObject image_prefab;

    protected void Start()
    {
        news_entry = (News_Entry)News.Selected_Data;
        Initialize();
    }

    protected virtual void Initialize()
    {
        Title.text = news_entry.Title;
        string month = news_entry.Creation_time.ToString("MMMM").ToUpper();
        Creation_time.text = month[0] + month[1] + month[2] + " " + news_entry.Creation_time.Day.ToString() + " de " + news_entry.Creation_time.Year;
        Detail.text = news_entry.Details;

        if (news_entry.Imgs[0] != "empty")
            foreach (string image in news_entry.Imgs)
                Http_Client.Download_Image(image, transform, Handle_Img_Response);

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();

        if (!news_entry.Seen)
        {
            string[] field_names = { "REQUEST_TYPE", "news_id" };
            string[] field_values = { "set_news_seen", news_entry.Id.ToString() };
            Http_Client.Send_Post(field_names, field_values, Handle_Response, Handler_Type.none);
            news_entry.Seen = true;
            User.User_Info.News_Data.Add(news_entry.Id);
        }
    }

    void Handle_Response(string response, Handler_Type type)
    {
    }

    void Handle_Img_Response(Transform parent, Texture2D texture)
    {
        GameObject new_Image = Instantiate(image_prefab, parent);
        RawImage raw_Image = new_Image.GetComponent<RawImage>();
        RectTransform rectTransform = new_Image.GetComponent<RectTransform>();

        raw_Image.texture = texture;

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();

        float ratio = (float)texture.height / (float)texture.width;

        Vector2 result = new Vector2(rectTransform.sizeDelta.x, (int)(rectTransform.sizeDelta.x * ratio));
        rectTransform.sizeDelta = result;
    }
}
