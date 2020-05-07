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
    GameObject edit_field_prefab;

    [SerializeField]
    GameObject edit_field_enum_prefab;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        Initialize(null);
    }

    public void Initialize(Data_struct data)
    {
        data = new Calendar_Event();
        ((Calendar_Event)data).Location_Event = "AGRUPAAAA";
        ((Calendar_Event)data).Votable_Type = Votable_Type.Yes_No;

        Data = data;

        FieldInfo[] properties = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (FieldInfo info in properties)
        {
            if (!data.Is_Editable(info))
                continue;

            Edit_Field field;

            if (info.FieldType.IsEnum)
                field = Instantiate(edit_field_enum_prefab, transform).GetComponent<Edit_Field_Enum>();
            else
                field = Instantiate(edit_field_prefab, transform).GetComponent<Edit_Field_Other>();

            field.Initialize(info);
        }
    }

    public

    // Update is called once per frame
    void Update()
    {
        
    }
}
