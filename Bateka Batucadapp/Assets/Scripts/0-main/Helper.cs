using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public static Helper Singleton;

    public Sprite Sprite_Event_Not_Answered;
    public Sprite Sprite_Event_Affirmed;
    public Sprite Sprite_Event_Rejected;
    public Sprite Sprite_Login;
    public Sprite Sprite_Login_Success;
    public Sprite Sprite_Login_Failure;
    public Sprite Sprite_Arrow_Seen;
    public Sprite Sprite_Event_Button_Affirmed;
    public Sprite Sprite_Event_Button_Rejected;

    private void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        TouchScreenKeyboard.hideInput = true;
    }

    public static Func<Image> Menu_Icon;
    public static Func<Sprite> Sprite_Red;

    public IEnumerator Update_Button()
    {
        while (Menu.Singleton == null)
            yield return null;

        Menu_Icon().sprite = Sprite_Red();

        yield return null;
    }
}
