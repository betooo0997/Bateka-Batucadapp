using System;
using System.Collections.Generic;

[System.Serializable]
public class Doc : Data_struct
{
    public Doc()
    {
        Imgs = new List<string>();
        Urls = new List<string>();
    }

    public DateTime Creation_time;
    public List<string> Urls;
    public List<string> Imgs;
}
