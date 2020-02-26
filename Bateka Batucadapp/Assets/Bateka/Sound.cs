using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void On_Time(object sender, EventArgs e)
    {
        source.PlayOneShot(source.clip);
        Debug.Log("fired");
    }
}