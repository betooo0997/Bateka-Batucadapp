#pragma warning disable 0649

using UnityEngine;
using Common.Cryptography;

public class Encryption
{
    static string db_key = "default";

    public static bool Has_Valid_Key()
    {
        return Decrypt("BbH5f62pH075sLrlPX80MQ==", db_key) == "abc";
    }

    public static string Encrypt(string text, string key = "")
    {
        string k = key == "" ? db_key : key;
        return Has_Valid_Key() ? AES.Encrypt(text, k) : text;
    }

    public static string Decrypt(string cipher, string key = "")
    {
        string k = key == "" ? db_key : key;
        Debug.Log("Decrypting " + cipher + " with key " + k);
        return AES.Decrypt(cipher, k);
    }

    public static void Get_DatabaseKey()
    {
        string key_lock = "aylmvdbsap33ng1q9sh" + SystemInfo.deviceUniqueIdentifier;
        string lock_pwd = Decrypt(PlayerPrefs.GetString("lock_pwd"), key_lock);

        string key = "ak3xb7awk8iasdkdHAYakqwr0fqs" + SystemInfo.deviceUniqueIdentifier + lock_pwd;
        db_key = AES.Decrypt(PlayerPrefs.GetString("db_key"), key);
    }
}
