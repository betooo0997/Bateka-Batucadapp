using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Rhythm_Loop : MonoBehaviour
{
    public List<Sound_Instance_Mono> Sound_Instances_Core;
    public List<Sound_Instance_Mono> Sound_Instances_Peripheric;
    public Sound_Data.Loop Data;
    public Sound_Type_Mono Sound;

    public Rhythm_Loop_Border[] Borders;

    public static Rhythm_Loop Selected;

    static Color color_selected;
    static Color color_not_selected;

    public float Get_Width_Core()
    {
        return Sound_Instances_Core.Count * (Rhythm_Player.Cell_Width + 2);
    }

    private void Awake()
    {
        Sound_Instances_Core = new List<Sound_Instance_Mono>();
        Sound_Instances_Peripheric = new List<Sound_Instance_Mono>();
        Borders = GetComponentsInChildren<Rhythm_Loop_Border>();
    }

    private void Start()
    {
        color_selected = new Color(0.2f, 0.2f, 0.2f);
        color_not_selected = new Color(0.2f, 0.2f, 0.2f, 0.5f);
    }

    public void Delete_Loop()
    {

        for (int x = 0; x < Sound_Instances_Core.Count; x++)
        {
            Sound_Instances_Core[x].transform.SetParent(transform.parent);
            Sound_Instances_Core[x].transform.SetSiblingIndex(transform.GetSiblingIndex() + 1 + x);
        }

        Destroy(gameObject);
    }

    public void Select()
    {
        Unselect();

        foreach (Rhythm_Loop_Border border in Borders)
            border.GetComponent<Image>().color = color_selected;

        Selected = this;
        Rhythm_Loop_Editor.Singleton.Show_In_Loop_Editor(this);
    }

    public static void Unselect()
    {
        if(Selected != null)
            foreach (Rhythm_Loop_Border border in Selected.Borders)
                border.GetComponent<Image>().color = color_not_selected;

        Selected = null;
    }

    public void Update_Core()
    {
        Sound_Instance_Mono[] already_cores = new Sound_Instance_Mono[Sound_Instances_Core.Count];
        Sound_Instances_Core.CopyTo(already_cores);
        Sound_Instances_Core = new List<Sound_Instance_Mono>();

        for (float y = Data.Start_Time; y <= Data.End_Time; y += 0.125f)
            Sound_Instances_Core.Add(Sound.Instances[y]);

        foreach (Sound_Instance_Mono instance in Sound_Instances_Core)
            if (!already_cores.ToList().Exists(a => a == instance))
                instance.Set_Repeated(null);

        foreach (Rhythm_Loop_Border border in Borders)
            border.Update_UI();

        transform.SetAsLastSibling();
    }

    public void Update_Periphery()
    {
        List<Sound_Instance_Mono> new_Peripheric = new List<Sound_Instance_Mono>();

        for (float x = 1; x <= Data.Repetitions; x++)
        {
            for (float y = 0; y < Data.Length_Steps; y++)
            {
                float key = y * 0.125f + Data.Start_Time + x * (Data.Length + 0.125f);
                Sound_Instance_Mono core = Sound.Instances[Data.Start_Time + y * 0.125f];
                Sound_Instance_Mono periphery = Sound.Instances[key];

                periphery.Set_Repeated(core);
                new_Peripheric.Add(periphery);
            }
        }

        foreach (Sound_Instance_Mono instance in Sound_Instances_Peripheric)
            if(!new_Peripheric.Exists(a => a == instance))
                    instance.Set_Repeated(null);

        Sound_Instances_Peripheric = new_Peripheric;
    }

    public void Change_Repetitions_Amount(uint repetitions_target)
    {
        List<Sound_Instance_Mono> cores = Sound_Instances_Core;
        List<Sound_Instance_Mono> peripheries = Sound_Instances_Peripheric;
        uint repetitions_current = Data.Repetitions;

        int loop_steps = Data.Length_Steps;

        if (repetitions_target < repetitions_current)
        {
            for (uint x = repetitions_current; x > repetitions_target; x--)
            {
                for (int y = loop_steps - 1; y >= 0; y--)
                {
                    Sound_Instance_Mono periphery = peripheries[(int)(x - 1) * loop_steps + y];
                    periphery.Set_Repeated(null);
                    peripheries.Remove(periphery);
                }
            }
        }
        else
        {
            for (uint x = repetitions_current + 1; x <= repetitions_target; x++)
            {
                for (int y = 0; y < loop_steps; y++)
                {
                    Sound_Instance_Mono periphery = Sound.Instances[((int)x * loop_steps + y) * Rhythm_Player.Singleton.Step];
                    Sound_Instance_Mono core = cores[y];

                    if (!periphery.Repeated)
                    {
                        periphery.Set_Repeated(core);
                        peripheries.Add(periphery);
                    }
                    else
                    {
                        Data.Repetitions = x;
                        Message.ShowMessage("Error, superposición entre bucles");
                        return;
                    }
                }
            }
        }

        Data.Repetitions = repetitions_target;
    }
}
