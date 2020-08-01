#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Edit_Handler : MonoBehaviour
{
    public static Edit_Handler Singleton;
    public static Data_struct Data;

    [SerializeField]
    GameObject edit_field_text_prefab, edit_field_enum_prefab, edit_field_date_prefab, edit_field_list_prefab;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Data = Database_Handler.Selected_Data;
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
        if (Data.GetType().Equals(typeof(Poll)))
        {
            Poll data = (Poll)Data;

            string[] field_names = {
                "REQUEST_TYPE",
                "poll_id",
                "poll_name",
                "poll_details",
                "poll_date_creation",
                "poll_date_deadline",
                "poll_author_id",
                "poll_privacy",
                "poll_options"
            };

            string[] field_values = {
                "set_poll",
                data.Id.ToString(),
                data.Title,
                data.Details,
                Utils.Get_String_SQL(data.Creation_Time),
                Utils.Get_String_SQL(data.Date_Deadline),
                data.Author_Id.ToString(),
                ((int)data.Privacy).ToString(),
                Utils.List_To_String(data.Vote_Types)
            };

            Http_Client.Send_Post(
                field_names,
                field_values,
                (string response, Handler_Type type) => {}
            );
        }
    }
}
