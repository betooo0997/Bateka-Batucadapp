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

    public Button Save_Button, Delete_Button;

    public static bool Deletable = true;
    public static Menu.Menu_item Prev;

    [SerializeField]
    GameObject edit_field_text_prefab, edit_field_enum_prefab, edit_field_date_prefab, edit_field_list_prefab;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        Delete_Button.gameObject.SetActive(Deletable);
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

        Delete_Button.transform.SetAsLastSibling();
    }

    public void Save()
    {
        List<string> field_names = new List<string>(){
            "REQUEST_TYPE",
        };

        List<string> field_values = new List<string>(){
            "",
            Data.Id.ToString(),
            Encryption.Encrypt(Data.Title),
            Encryption.Encrypt(Data.Details),
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
                Encryption.Encrypt(data.Location_Event),
                Encryption.Encrypt(data.Location_Meeting),
                Utils.Get_String_SQL(data.Date_Event),
                Utils.Get_String_SQL(data.Date_Meeting),
                Utils.Get_String_SQL(data.Date_Deadline),
                Encryption.Encrypt(data.Transportation),
                Encryption.Encrypt(data.Cash),
                Encryption.Encrypt(data.Food)
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
                Encryption.Encrypt(Utils.List_To_String(data.Imgs))
            });
        }

        else
            return;

        Database_Handler.Selected_Data = Data;

        if(Data.Id == 0)
            Database_Handler.Data_List_Add(Data.Database_Handler_Type(), Data);

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

        Delete_Button.gameObject.SetActive(true);
        Message.ShowMessage("Archivo actualizado con éxito.");
    }

    public void Delete()
    {
        string[] field_names = { "REQUEST_TYPE", "data_id"};
        string[] field_values;

        if (Data.GetType().Equals(typeof(Poll)))
            field_values = new string[]{ "delete_poll", Data.Id.ToString() };

        else if (Data.GetType().Equals(typeof(Calendar_Event)))
            field_values = new string[] { "delete_event", Data.Id.ToString() };

        else if (Data.GetType().Equals(typeof(News_Entry)))
            field_values = new string[] { "delete_news", Data.Id.ToString() };

        else
            return;


        Notification_UI_Pop.Show_Message(
            "Confirmar eliminación", 
            "Seguro que quieres eliminar este archivo?", 
            () => 
            {
                Type type = Data.Database_Handler_Type();
                List<Data_struct> data_list = Database_Handler.Data_List_Get(type);
                data_list.Remove(data_list.Find(a => a.Id == Data.Id));
                Database_Handler.Data_List_Set(type, data_list);

                Http_Client.Send_Post(
                    field_names,
                    field_values,
                    Handle_Delete_Response
                );

                Menu.Singleton.Load_Scene_Menu_Item(Prev);
            }, 
            "Eliminar", 
            "Cancelar");
    }

    void Handle_Delete_Response(string response, Handler_Type type)
    {
        if (response.Contains("500"))
        {
            Message.ShowMessage("Error interno del servidor.");
            return;
        }

        Message.ShowMessage("Archivo elminiado con éxito.");
    }
}
