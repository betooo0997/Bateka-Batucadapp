using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rhythm
{
    [System.Serializable]
    public class Sound
    {
        public enum Sound_Type
        {
            None,
            Surdo_20,
            Surdo_18,
            Surdo_16,
            Tabal
        }

        [System.Serializable]
        public struct Instance
        {
            public float Fire_Time;
            public float Volume;
            public string Note;
        }

        public Sound_Type Type;
        public List<Instance> Instances;

        public Sound()
        {
            Instances = new List<Instance>();
        }
    }

    public uint Id;
    public DateTime Last_Modification_Date;
    public List<Sound> Sounds;

    public  Rhythm()
    {
        Sounds = new List<Sound>();
    }
}
