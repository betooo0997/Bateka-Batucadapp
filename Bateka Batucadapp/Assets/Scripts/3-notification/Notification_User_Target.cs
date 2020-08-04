using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification_User_Target : MonoBehaviour
{
    public User.User_Information User;

    [SerializeField]
    Text name;

    public void Initialize(string name)
    {
        this.name.text = name;
    }

    public void Delete_Target()
    {
        Notification_User_Searcher.Singleton.Targets.Remove(User);
        Destroy(gameObject);
        Utils.Update_UI = true;
    }
}
