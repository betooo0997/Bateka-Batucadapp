using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Database_Handlers : MonoBehaviour
{
    static Dictionary<Type, Database_Handlers> singletons;
    static Dictionary<Type, Data_structs> selected_data;

    public static void Initialize_Dictionaries()
    {
        singletons = new Dictionary<Type, Database_Handlers>();
        selected_data = new Dictionary<Type, Data_structs>();
    }

    protected void Awake()
    {
        singletons.Add(GetType(), this);
    }

    void OnDestroy()
    {
        singletons.Remove(GetType());
    }

    public static Database_Handlers Get_Singleton(Type type)
    {
        return singletons[type];
    }

    public static bool Singleton_Exists(Type type)
    {
        return singletons.ContainsKey(type);
    }

    public static Data_structs Get_Selected_data(Type type)
    {
        return selected_data[type];
    }

    public static void Set_Selected_data(Data_structs selected)
    {
        Type key = selected.GetType();

        if (singletons.ContainsKey(key))
            selected_data[key] = selected;
        else
            selected_data.Add(key, selected);
    }


    public static void Set_Singleton(Database_Handlers instance)
    {
        Type key = instance.GetType();

        if (singletons.ContainsKey(key))
            singletons[key] = instance;
        else
            singletons.Add(key, instance);
    }
}
