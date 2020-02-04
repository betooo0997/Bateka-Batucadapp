using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Data_UI : MonoBehaviour
{
    public virtual void Set_Values(Data_struct data) { }

    protected Color color_not_answered(float a) { return new Color(1f, 0.83f, 0f, a); }
    protected Color color_affirmed(float a) { return new Color(0.5f, 1, 0.5f, a); }
    protected Color color_rejected(float a) { return new Color(1f, 0.5f, 0.5f, a); }
    protected Color color_other(float a) { return color_affirmed(a); }
}
