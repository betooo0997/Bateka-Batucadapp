using System;
using System.Collections.Generic;

[System.Serializable]
public class Doc : Data_struct
{
    public Doc()
    {
        Imgs = new List<string>();
        Content_URLs = new List<string>();
    }

    public string Subtitle;
    public DateTime Creation_time;
    public List<string> Content_URLs;
    public List<string> Imgs;
}
