using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Doc_UI_summarized : Doc_UI
{
    protected Image background;

    protected void Awake()
    {
        background = GetComponent<Image>();
    }

    public override void Set_Data (Data_struct doc)
    {
        Doc = (Doc)doc;
        Title.text = Doc.Title;
        Date.text = Utils.Get_String(Doc.Creation_time);
    }
}
