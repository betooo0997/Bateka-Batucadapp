using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }
}
