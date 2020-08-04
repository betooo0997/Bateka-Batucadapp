using System.Collections;
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
    Transform target_parent;

    [SerializeField]
    GameObject target_prefab;

    public void Add_Target(User_Information user)
    {
        Notification_User_Target target = Instantiate(target_prefab, target_parent).GetComponent<Notification_User_Target>();
        target.User = user;
    }
}
