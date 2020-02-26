using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Instance : MonoBehaviour
{
    [System.NonSerialized]
    public float Fire_Time;

    [System.NonSerialized]
    public Sound sound;

    [SerializeField]
    Image image;

    bool enabled;

    public void Load()
    {
        if (enabled)
        {
            float key = Rythm_Player.Round_To_Existing_Key(Fire_Time);
            Rythm_Player.Singleton.Time_Events[key] += sound.On_Time;
        }
    }

    public void Toggle_Enable()
    {
        enabled = !enabled;
        image.enabled = enabled;
    }
}