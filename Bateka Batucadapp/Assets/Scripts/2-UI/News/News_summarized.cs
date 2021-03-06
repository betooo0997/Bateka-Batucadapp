﻿#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News_summarized : News_UI
{
    [SerializeField]
    Image image;

    public override void Set_Data(Data_struct news_entry)
    {
        this.news_entry = (News_Entry)news_entry;
        Title.text = this.news_entry.Title.ToUpper();
        Creation_time.text = this.news_entry.Creation_time.ToString("MMMM").ToUpper() + " " + this.news_entry.Creation_time.Day;

        if (this.news_entry.Seen)
            image.sprite = Helper.Singleton.Sprite_Arrow_Seen;
    }
}
