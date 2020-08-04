#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Edit_Handler : MonoBehaviour
{
    public static Edit_Handler Singleton;
    public static Data_struct Data;

    public Button Save_Button;

    [SerializeField]
    GameObject edit_field_text_prefab, edit_field_enum_prefab, edit_field_date_prefab, edit_field_list_prefab;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Data = Database_Handler.Selected_Data.Deep_Copy();
        Initialize();
    }

    public void Initialize()
    {
        FieldInfo[] properties = Data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo info in properties)
        {
            if (!Data.Is_Editable(info))
                continue;

            Edit_Field field;

            if (info.FieldType.IsEnum)
                field = Instantiate(edit_field_enum_prefab, transform).GetComponent<Edit_Field_Enum>();

            else if (info.FieldType == typeof(List<string>))
                field = Instantiate(edit_field_list_prefab, transform).GetComponent<Edit_Field_List>();

            else
            {
                switch (info.FieldType.ToString().ToLower())
                {
                    case "system.datetime":
                        field = Instantiate(edit_field_date_prefab, transform).GetComponent<Edit_Field_DateTime>();
                        break;

                    default:
                        field = Instantiate(edit_field_text_prefab, transform).GetComponent<Edit_Field_Text>();
                        break;
                }
            }

            field.Initialize(info);
        }
    }

    public void Save()
    {
        List<string> field_names = new List<string>(){
            "REQUEST_TYPE",
        };

        List<string> field_values = new List<string>(){
            "",
            Data.Id.ToString(),
            Data.Title,
            Data.Details,
            Data.Author_Id.ToString(),
            Data.Privacy.ToString()
        };

        if (Data.GetType().Equals(typeof(Poll)))
        {
            Poll data = (Poll)Data;
            field_names.AddRange(new List<string>(){
                "poll_id",
                "poll_name",
                "poll_details",
                "poll_author_id",
                "poll_privacy",
                "poll_date_creation",
                "poll_date_deadline",
                "poll_options"
            });

            field_values[0] = "set_poll";
            field_values.AddRange(new List<string>(){
                Utils.Get_String_SQL(data.Creation_Time),
                Utils.Get_String_SQL(data.Date_Deadline),
                Utils.List_To_String(data.Vote_Types)
            });
        }

        else if (Data.GetType().Equals(typeof(Calendar_Event)))
        {
            Calendar_Event data = (Calendar_Event)Data;
            field_names.AddRange(new List<string>(){
                "event_id",
                "event_name",
                "event_details",
                "event_author_id",
                "event_privacy",
                "event_location_event",
                "event_location_meeting",
                "event_date_event",
                "event_date_meeting",
                "event_date_deadline",
                "event_transportation",
                "event_cash",
                "event_food"
            });

            field_values[0] = "set_event";
            field_values.AddRange(new List<string>(){
                data.Location_Event,
                data.Location_Meeting,
                Utils.Get_String_SQL(data.Date_Event),
                Utils.Get_String_SQL(data.Date_Meeting),
                Utils.Get_String_SQL(data.Date_Deadline),
                data.Transportation,
                data.Cash,
                data.Food
            });
        }

        else if (Data.GetType().Equals(typeof(News_Entry)))
        {
            News_Entry data = (News_Entry)Data;
            field_names.AddRange(new List<string>(){
                "news_id",
                "news_name",
                "news_details",
                "news_author_id",
                "news_privacy",
                "news_date_creation",
                "news_images"
            });

            field_values[0] = "set_news";
            field_values.AddRange(new List<string>(){
                Utils.Get_String_SQL(data.Creation_time),
                Utils.List_To_String(data.Imgs)
            });
        }

        else
            return;

        Database_Handler.Selected_Data = Data;

        Http_Client.Send_Post(
            field_names.ToArray(),
            field_values.ToArray(),
            Handle_Save_Response
        );

        Save_Button.interactable = false;
    }

    void Handle_Save_Response(string response, Handler_Type type)
    {
        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        Message.ShowMessage("Archivo actualizado con éxito.");
    }
}
