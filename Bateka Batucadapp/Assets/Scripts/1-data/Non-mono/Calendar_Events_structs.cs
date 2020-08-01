using System;

[System.Serializable]
public class Calendar_Event : Votable
{
    public string Status;
    public DateTime Date_Event;
    public DateTime Date_Meeting;
    public string Location_Event;
    public string Location_Meeting;
    public string Transportation;
    public string Cash;
    public string Food;

    public Calendar_Event() : base()
    {
        Votable_Type        = Votable_Type.Binary;
        Date_Event          = new DateTime();
        Date_Meeting        = new DateTime(); ;
        Location_Event      = "";
        Location_Meeting    = "";
        Status              = "";
        Transportation      = "";
        Cash                = "";
        Food                = "";

        editable.Add("Location_Event");
        editable.Add("Date_Event");
        editable.Add("Date_Meeting");
        editable.Add("Location_Meeting");
        editable.Add("Transportation");
        editable.Add("Cash");
        editable.Add("Food");
    }
}
