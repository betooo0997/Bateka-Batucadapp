using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sound : MonoBehaviour
{
    public static List<Sound> Sounds;
    public Dictionary<float, Sound_Instance> Instances;
    public static Sound Interactable_Sound;
    public Rhythm.Sound.Sound_Type Sound_Type;

    public List<Rhythm_Loop> Loops;

    [SerializeField]
    Text title;

    AudioSource source;

    void Awake()
    {
        if (Sounds == null)
            Sounds = new List<Sound>();
        source = GetComponent<AudioSource>();
        Instances = new Dictionary<float, Sound_Instance>();
        Loops = new List<Rhythm_Loop>();
    }

    private void Start()
    {
        Sounds.Add(this);
        title.text = Sound_Type.ToString();
    }

    public void On_Time(object sender, EventArgs e)
    {
        source.PlayOneShot(source.clip);
    }

    public void Toggle_Edit()
    {
        if (Interactable_Sound != this)
        {
            if (Interactable_Sound != null)
            {
                foreach (Sound_Instance instance in Interactable_Sound.Instances.Values.ToList())
                {
                    instance.Button.interactable = false;

                    RectTransform rect_transform = instance.sound.title.transform.parent.GetComponent<RectTransform>();
                    rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, 45);

                    rect_transform = instance.sound.GetComponent<RectTransform>();
                    rect_transform.sizeDelta = new Vector2(rect_transform.sizeDelta.x, 45);
                }

                Interactable_Sound.Update_Loop_Borders();
            }

            Interactable_Sound = this;

            foreach (Sound_Instance instance in Instances.Values.ToList())
            {
                instance.Button.interactable = true;
                Set_Height(instance, 60);
            }
        }
        else
        {
            foreach (Sound_Instance instance in Interactable_Sound.Instances.Values.ToList())
            {
                instance.Button.interactable = false;
                Interactable_Sound = null;
                Set_Height(instance, 45);
            }
        }

        Update_Loop_Borders();

        foreach (HorizontalOrVerticalLayoutGroup layout in FindObjectsOfType<HorizontalOrVerticalLayoutGroup>())
        {
            layout.SetLayoutVertical();
            layout.SetLayoutHorizontal();
        }

        Canvas.ForceUpdateCanvases();
    }

    void Set_Height(Sound_Instance instance, float height)
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