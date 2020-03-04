using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rhythm_Loop_Editor : MonoBehaviour
{
    public static Rhythm_Loop_Editor Singleton;

    [SerializeField]
    GameObject parent;

    [SerializeField]
    InputField Repetitions;

    void Awake()
    {
        Singleton = this;
        parent.SetActive(false);
    }

    public void Show_In_Loop_Editor(Rhythm_Loop loop)
    {
        parent.SetActive(true);
        Repetitions.text = loop.Data.Repetitions.ToString();
    }

    public void Change_Repetitions_Amount(string repetitions_string)
    {
        Rhythm_Loop.Selected.Change_Repetitions_Amount(uint.Parse(repetitions_string));
    }

    public void Delete_Loop()
    {

    }
}
