﻿using System;
using System.Collections.Generic;

[System.Serializable]
public class News_Entry : Data_struct
{
    public News_Entry() : base()
    {
        Imgs = new List<string>();
        Creation_time = DateTime.UtcNow;
    }

    public DateTime Creation_time;
    public List<string> Imgs;
    public bool Seen;
}
