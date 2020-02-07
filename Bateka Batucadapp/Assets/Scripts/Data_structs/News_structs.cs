using System;
using System.Collections.Generic;

[System.Serializable]
public class News_Entry : Data_struct
{
    public News_Entry()
    {
        Img_URLs = new List<string>();
    }

    public DateTime Creation_time;
    public List<string> Img_URLs;
}
