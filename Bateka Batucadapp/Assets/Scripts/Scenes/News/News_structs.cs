using System.Collections.Generic;

[System.Serializable]
public class News_Entry
{
    public News_Entry()
    {
        Img_URLs = new List<string>();
    }

    public int Id;
    public string Title;
    public string Details;
    public string Creation_time;
    public List<string> Img_URLs;
}
