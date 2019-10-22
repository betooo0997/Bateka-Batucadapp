using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class User
{
    public enum User_Role
    {
        default_,
        admin
    }

    public static string Username;
    public static string Psswd;
    public static User_Role Role;
    public static string Name;
    public static string Surname;
    public static string Email;
    public static string Tel;
}
