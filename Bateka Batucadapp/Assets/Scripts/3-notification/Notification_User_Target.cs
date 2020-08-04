#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static User;

public class Notification_User_Target : MonoBehaviour
{
    public User_Information User;

    [SerializeField]
    Text target_name;

    public void Initialize(User_Information info)
    {
        User = info;
        target_name.text = info.Name + " (" + info.Username + ")";
    }

    public void Delete_Target()
    {
        Notification_User_Searcher.Singleton.Targets.Remove(User);
        Destroy(gameObject);
        Utils.Update_UI = true;
    }
}
