using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class News_UI : MonoBehaviour
{
    public Text Title;
    public Text Creation_time;

    News_Entry news_entry;

    private void Start()
    {
        Title.text = news_entry.Title;
        Creation_time.text = news_entry.Creation_time;
    }
}
