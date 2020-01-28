using System.Collections.Generic;

[System.Serializable]
public class News_Entry : Data_structs
{
    public News_Entry()
    {
        Img_URLs = new List<string>();
    }

    public string Creation_time;
    public List<string> Img_URLs;
}
