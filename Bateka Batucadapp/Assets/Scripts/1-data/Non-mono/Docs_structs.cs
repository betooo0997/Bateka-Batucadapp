﻿using System;
using System.Collections.Generic;

[System.Serializable]
public class Doc : Data_struct
{
    public Doc() : base()
    {
        Imgs = new List<string>();
        Urls = new List<string>();
        Creation_time = DateTime.UtcNow;
    }

    public DateTime Creation_time;
    public List<string> Urls;
    public List<string> Imgs;
}
