#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Poll_multiselect_option : MonoBehaviour
{
    public Button Button;

    [SerializeField]
    Image button_background, checkmark;

    [SerializeField]
    Text description;

    Poll poll;
}
