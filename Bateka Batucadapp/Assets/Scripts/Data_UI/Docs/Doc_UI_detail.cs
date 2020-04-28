using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Doc_UI_detail : Doc_UI
{
    [SerializeField]
    Text details;

    [SerializeField]
    GameObject image_prefab;

    [SerializeField]
    GameObject url_prefab;

    void Start()
    {
        Doc = (Doc)Docs.Selected_Data;
        Initialize();
    }

    protected virtual void Initialize()
    {
        Title.text = Doc.Title;
        Date.text = Utils.Get_String(Doc.Creation_time);
        details.text = Doc.Details;

        foreach (string url in Doc.Urls)
        {
            Button button = Instantiate(url_prefab, transform).GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                Message.ShowMessage("Enlace copiado, abriéndolo en el navegador.");
                GUIUtility.systemCopyBuffer = url;
                Application.OpenURL(url);
            });

            button.GetComponentInChildren<Text>().text = url;
        }

        Date.transform.SetAsLastSibling();

        if (Doc.Imgs[0] != "empty")
            foreach (string image in Doc.Imgs)
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
        Date.transform.SetAsLastSibling();
    }
}
