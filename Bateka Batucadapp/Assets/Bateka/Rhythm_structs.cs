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

        public class Loop
        {
            public float Start_Time;
            public float End_Time;
            public float Length { get { return End_Time - Start_Time; } set { Start_Time = End_Time - value; } }
            public int Length_Steps { get { return (int)(Length / Rhythm_Player.Singleton.Step) + 1; } set { Start_Time = End_Time - value * Rhythm_Player.Singleton.Step; } }
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
    public string Title;
    public string Description;
    public uint Author_id;
    public DateTime Last_Update;
    public DateTime Creation;

    public List<Sound> Sounds;

    public  Rhythm()
    {
        Sounds = new List<Sound>();
    }

    public string Get_Sounds_Json()
    {
        string result = "{";

        foreach (Sound.Sound_Type sound_type in (Sound.Sound_Type[])Enum.GetValues(typeof(Sound.Sound_Type)))
        {
            if (sound_type == Rhythm.Sound.Sound_Type.None)
                continue;

            Sound sound = Sounds.Find(a => a.Type == sound_type);
            result += "\"" + sound_type.ToString() + "\":[[";

            foreach (Sound.Instance instance in sound.Instances)
                result += "\"" + Utils.ToString(instance.Fire_Time) + "*" + Utils.ToString(instance.Volume) + "*" + instance.Note + "\",";

            if(sound.Instances.Count > 0)
                result = result.Substring(0, result.Length - 1);

            result += "],[";

            foreach (Sound.Loop loop in sound.Loops)
                result += "\"" + Utils.ToString(loop.Start_Time) + "*" + Utils.ToString(loop.End_Time) + "*" + loop.Repetitions + "\",";

            if (sound.Loops.Count > 0)
                result = result.Substring(0, result.Length - 1);

            result += "]],";
        }

        return result.Substring(0, result.Length - 1) + "}";
    }
}

public class Sound_Raw
{
    public string[][] Surdo_20;
    public string[][] Surdo_18;
    public string[][] Surdo_16;
    public string[][] Tabal;
}
