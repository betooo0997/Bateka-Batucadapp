#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static User;

public class Notification_User_Searcher : MonoBehaviour
{
    public static Notification_User_Searcher Singleton;

    [System.NonSerialized]
    public List<User_Information> Targets;

    [SerializeField]
    InputField search_field;

    [SerializeField]
    Transform suggested_parent;

    [SerializeField]
    GameObject suggested_prefab;

    [SerializeField]
    Transform target_parent;

    [SerializeField]
    GameObject target_prefab;

    bool block;

    void Awake()
    {
        Targets = new List<User_Information>();
    }

    void Add_Suggested(User_Information info)
    {
        Notification_User_Target target = Instantiate(suggested_prefab, suggested_parent).GetComponent<Notification_User_Target>();
        target.Initialize(info);
        target.GetComponent<Button>().onClick.AddListener(() => { Add_Target(target); });
    }

    void Destroy_Suggested()
    {
        for (int x = suggested_parent.childCount - 1; x >= 0; x--)
            Destroy(suggested_parent.GetChild(x).gameObject);
    }

    void Destroy_Target(Notification_User_Target target)
    {
        Targets.Remove(target.User);
        target_parent.gameObject.SetActive(false);
        Destroy(target.gameObject);
        Utils.InvokeNextFrame(() => { target_parent.gameObject.SetActive(true); });
    }

    public void Add_Target(Notification_User_Target suggested)
    {
        Destroy_Suggested();

        Notification_User_Target target = Instantiate(target_prefab, target_parent).GetComponent<Notification_User_Target>();
        target.Initialize(suggested.User);
        target.GetComponentInChildren<Button>().onClick.AddListener(() => { Destroy_Target(target); });

        block = true;
        search_field.text = "";
        block = false;

        Targets.Add(suggested.User);
        target_parent.gameObject.SetActive(false);
        Utils.InvokeNextFrame(() => { target_parent.gameObject.SetActive(true); });
    }

    public void On_Value_Change(string value)
    {
        Destroy_Suggested();

        Utils.InvokeNextFrame(() =>
        {
            if (!block && value.Length >= 2)
            {
                foreach(User_Information info in Users_Info)
                {
                    if(info.Name.ToLower().Contains(value) || info.Username.ToLower().Contains(value))
                    {
                        Add_Suggested(info);
                    }
                }
            }

            Utils.Update_UI = true;
        });
    }
}
