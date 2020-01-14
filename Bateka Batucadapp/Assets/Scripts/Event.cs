using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Event : MonoBehaviour
{
    [SerializeField]
    GameObject events_Prefab;

    [SerializeField]
    Transform event_parent;

    [System.Serializable]
    public struct Event_Information
    {
        public string Title;
        public string Location;
        public string Date;
        public string Detail;
        public string Confirm_Deadline;
    }
}
