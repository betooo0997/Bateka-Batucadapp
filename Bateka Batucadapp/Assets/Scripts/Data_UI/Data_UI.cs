using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data_UI : MonoBehaviour
{
    public virtual void Set_Data(Data_struct data) { }

    public static Color color_not_answered(float a) { return new Color(1f, 0.83f, 0f, a); }
    public static Color color_affirmed(float a) { return new Color(0.5f, 1, 0.5f, a); }
    public static Color color_rejected(float a) { return new Color(1f, 0.5f, 0.5f, a); }
    public static Color color_other(float a) { return color_affirmed(a); }
    public static Color color_alternative_0(float a) { return new Color(0.5f, 0.5f, 1f, a); }
    public static Color color_alternative_1(float a) { return new Color(1f, 0.5f, 1f, a); }
    public static Color color_alternative_2(float a) { return new Color(0.5f, 1f, 1f, a); }
    public static Color color_alternative_3(float a) { return new Color(1f, 1f, 0.5f, a); }
}
