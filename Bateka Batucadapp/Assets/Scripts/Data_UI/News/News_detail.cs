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
        Creation_time.text = Utils.Get_String(news_entry.Creation_time);
        Detail.text = news_entry.Details;

        foreach (string image in news_entry.Img_URLs)
            Http_Client.Download_Image(image, transform, Handle_Img_Response);

        Canvas.ForceUpdateCanvases();
        GetComponentInChildren<VerticalLayoutGroup>().SetLayoutVertical();
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
        Creation_time.transform.SetAsLastSibling();
    }
}
