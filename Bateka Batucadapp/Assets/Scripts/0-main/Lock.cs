#pragma warning disable 0649

using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Lock : MonoBehaviour
{
    public static Lock Singleton;

    static bool first_lock = true;

    [SerializeField]
    Text input, new_pwd, repeat_pwd;

    [SerializeField]
    Button save_button;

    [SerializeField]
    Image background;

    string pwd = "";
    string pwd_created = "";
    bool creating_pwd = false;

    string[] black_list = { "1234", "147#", "2580", "3699", "0000", "1111", "2222", "3333",
                "4444", "5555", "6666", "7777", "8888", "9999", "####" };

    private void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        Utils.Adapt_Background(background);

        if (!PlayerPrefs.HasKey("lock_pwd"))
        {
            save_button.gameObject.SetActive(true);
            new_pwd.gameObject.SetActive(true);
            creating_pwd = true;
        }
    }

    public void On_NumPad_Press(string data)
    {
        string to_show = "";

        if (data == "delete")
        {
            if (pwd.Length > 0)
            {
                pwd = pwd.Substring(0, pwd.Length - 1);

                if (pwd.Length > 0)
                    to_show = "*";
            }
        }
        else
        {
            pwd += data;

            string key = "aylmvdbsap33ng1q9sh" + SystemInfo.deviceUniqueIdentifier;

            string pwd_saved ()
            {
                return  Encryption.Decrypt(PlayerPrefs.GetString("lock_pwd"), key);
            }

            if (!creating_pwd && PlayerPrefs.HasKey("lock_pwd") && pwd == pwd_saved()
                || creating_pwd && pwd == pwd_created && !black_list.Contains(pwd_created))
            {
                gameObject.SetActive(false);

                if (creating_pwd)
                    PlayerPrefs.SetString("lock_pwd", Encryption.Encrypt(pwd, key));

                int lock_index = SceneManager.GetSceneByName("Lock").buildIndex;

                if (first_lock)
                {
                    Debug.LogWarning("Unloading Lock Scene. First.");
                    SceneManager.LoadScene("Login");
                    Utils.Unload_Time(lock_index, 0.5f);
                    first_lock = false;
                }
                else
                {
                    Debug.LogWarning("Unloading Lock Scene.");
                    SceneManager.UnloadSceneAsync(lock_index);
                }
            }

            to_show = pwd.Substring(pwd.Length - 1, 1);
        }

        for (int x = 0; x < pwd.Length - 1; x++)
            to_show = "*" + to_show;

        input.text = to_show;
    }

    public void Save_Password()
    {
        if (!black_list.Contains(pwd))
        {
            pwd_created = pwd;
            pwd = "";
            input.text = pwd;
            repeat_pwd.gameObject.SetActive(true);
            new_pwd.gameObject.SetActive(false);
        }
    }
}
