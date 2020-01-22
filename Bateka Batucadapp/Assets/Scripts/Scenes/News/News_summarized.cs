using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class News_summarized : News_UI
{
    public void Set_Values(News_Entry news_entry)
    {
        this.news_entry = news_entry;
        Title.text = news_entry.Title;
        Creation_time.text = news_entry.Creation_time;
    }
}
