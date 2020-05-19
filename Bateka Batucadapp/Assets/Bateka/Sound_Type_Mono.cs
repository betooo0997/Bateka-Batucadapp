using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Type_Mono : MonoBehaviour
{
    public static List<Sound_Type_Mono> Sounds;
    public static Sound_Type_Mono Selected_Sound;

    public Dictionary<float, Sound_Instance_Mono> Instances;
    public Sound_Data.Sound_Type Sound_Type;

    public List<Rhythm_Loop> Loops;

    [SerializeField]
    Text title;

    AudioSource source;

    void Awake()
    {
        if (Sounds == null)
            Sounds = new List<Sound_Type_Mono>();
        source = GetComponent<AudioSource>();
        Instances = new Dictionary<float, Sound_Instance_Mono>();
        Loops = new List<Rhythm_Loop>();
    }

    private void Start()
    {
        Sounds.Add(this);
        title.text = Sound_Type.ToString();
    }

    public void On_Time(object sender, EventArgs e)
    {
        source.PlayOneShot(source.clip, Instances[Rhythm_Player.Singleton.Timer_Key].Volume);
    }

    public void Toggle_Edit()
    {
        if (Selected_Sound != this)
        {
            if (Selected_Sound != null)
            {
                foreach (Sound_Instance_Mono instance in Selected_Sound.Instances.Values.ToList())
                {
                    RectTransform rect_transform = instance.sound.title.transform.parent.GetComponent<RectTransform>();
                    rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, 45);

                    rect_transform = instance.sound.GetComponent<RectTransform>();
                    rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, 45);

                    instance.Button.interactable = false;
                    Utils.InvokeNextFrame(() => instance.Update_Volume());
                }

                Selected_Sound.Update_Loop_Borders();
            }

            Selected_Sound = this;

            foreach (Sound_Instance_Mono instance in Instances.Values.ToList())
            {
                instance.Button.interactable = true;
                Set_Height(instance, 60);
                Utils.InvokeNextFrame(() => instance.Update_Volume());
            }
        }
        else
        {
            foreach (Sound_Instance_Mono instance in Selected_Sound.Instances.Values.ToList())
            {
                instance.Button.interactable = false;
                Selected_Sound = null;
                Set_Height(instance, 45);
                Utils.InvokeNextFrame(() => instance.Update_Volume());
            }
        }

        Update_Loop_Borders();
        Utils.Update_UI = true;
    }

    void Set_Height(Sound_Instance_Mono instance, float height)
    {
        RectTransform rect_transform = instance.sound.title.transform.parent.GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, height);

        rect_transform = GetComponent<RectTransform>();
        rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, height);
    }

    public void Update_Loop_Borders()
    {
        foreach (Rhythm_Loop loop in Loops)
            foreach (Rhythm_Loop_Border border in loop.Borders)
                border.Update_UI();
    }
}