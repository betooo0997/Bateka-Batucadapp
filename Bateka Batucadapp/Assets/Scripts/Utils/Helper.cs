using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    public static Helper Singleton;

    public Sprite Sprite_Event_Not_Answered;
    public Sprite Sprite_Event_Affirmed;
    public Sprite Sprite_Event_Rejected;
    public Sprite Sprite_Login;
    public Sprite Sprite_Login_Success;
    public Sprite Sprite_Login_Failure;

    private void Awake()
    {
        Singleton = this;
    }
}
