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

        [System.Serializable]
        public struct Loop
        {
            public float Start_Time;
            public float End_Time;
            public uint Repetitions;
        }

        public Sound_Type Type;
        public List<Instance> Instances;
        public List<Loop> Loops;

        public Sound()
        {
            Instances = new List<Instance>();
            Loops = new List<Loop>();
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
