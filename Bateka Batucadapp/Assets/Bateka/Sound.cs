using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public static List<Sound> Sounds;
    public Dictionary<float, Sound_Instance> Instances;

    [SerializeField]
    public Rhythm.Sound.Sound_Type Sound_Type;

    AudioSource source;

    void Awake()
    {
        if (Sounds == null)
            Sounds = new List<Sound>();
        source = GetComponent<AudioSource>();
        Instances = new Dictionary<float, Sound_Instance>();
    }

    private void Start()
    {
        Sounds.Add(this);
    }

    public void On_Time(object sender, EventArgs e)
    {
        source.PlayOneShot(source.clip);
    }
}