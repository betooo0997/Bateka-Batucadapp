using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Rhythm_Data
{
    public uint Id;
    public string Name;
    public string Description;
    public uint PPM;
    public uint Author_id;
    public DateTime Last_Update;
    public DateTime Creation;

    public List<Sound_Data> Sounds_Data;

    public  Rhythm_Data()
    {
        Sounds_Data = new List<Sound_Data>();
    }

    public string Get_Sounds_Json()
    {
        string result = "{";

        foreach (Sound_Data.Sound_Type sound_type in (Sound_Data.Sound_Type[])Enum.GetValues(typeof(Sound_Data.Sound_Type)))
        {
            if (sound_type == Sound_Data.Sound_Type.None)
                continue;

            Sound_Data sound = Sounds_Data.Find(a => a.Type == sound_type);
            result += "\"" + sound_type.ToString() + "\":[[";

            foreach (Sound_Data.Instance instance in sound.Instances)
                result += "\"" + Utils.ToString(instance.Fire_Time) + "*" + Utils.ToString(instance.Volume) + "*" + instance.Note + "\",";

            if(sound.Instances.Count > 0)
                result = result.Substring(0, result.Length - 1);

            result += "],[";

            foreach (Sound_Data.Loop loop in sound.Loops)
                result += "\"" + Utils.ToString(loop.Start_Time) + "*" + Utils.ToString(loop.End_Time) + "*" + loop.Repetitions + "\",";

            if (sound.Loops.Count > 0)
                result = result.Substring(0, result.Length - 1);

            result += "]],";
        }

        return result.Substring(0, result.Length - 1) + "}";
    }
}

[System.Serializable]
public class Sound_Data
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
    public class Instance
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

    public Sound_Data()
    {
        Instances = new List<Instance>();
        Loops = new List<Loop>();
    }
}
