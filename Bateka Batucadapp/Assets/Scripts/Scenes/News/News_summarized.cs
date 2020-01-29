using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News_summarized : News_UI
{
    public override void Set_Values(Data_struct news_entry)
    {
        this.news_entry = (News_Entry)news_entry;
        Title.text = this.news_entry.Title;
        Creation_time.text = this.news_entry.Creation_time;
    }
}
