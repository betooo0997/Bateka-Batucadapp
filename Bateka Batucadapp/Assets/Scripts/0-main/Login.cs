#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public static Login Singleton;

    public Button Login_Button;

    [SerializeField]
    InputField user, password, password_db;

    [SerializeField]
    Image background, login_reponse;

    [SerializeField]
    GameObject login_loading;

    bool response, success;

    float timer;

    string temp_psswd;

    void Awake()
    {
        Singleton = this;

        #if UNITY_EDITOR
            EditorApplication.pauseStateChanged += LogPauseState;
        #endif
    }

    void Start()
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Login"));
        Utils.Adapt_Background(background);

        if (PlayerPrefs.HasKey("db_key"))
        {
            password_db.text = "**********";
            password_db.interactable = false;
        }
    }

    void Update()
    {
        if(response)
        {
            if(login_reponse.color.a < 1)
                login_reponse.color += new Color(0, 0, 0, Time.deltaTime * 1.5f);
            else
            {
                timer += Time.deltaTime;

                if(timer > 1.5f)
                {
                    timer = 0;
                    login_loading.SetActive(false);
                    Login_Button.interactable = true;

                    if (success)
                        Utils.Load_Scene_ST("Menu");

                    response = false;
                    Invoke("Reset_Login_Button", 2);
                }
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("Pause status: " + pauseStatus);

        if (pauseStatus && Lock.Singleton == null)
        {
            Debug.Log("Locking");
            SceneManager.LoadSceneAsync("Lock", LoadSceneMode.Additive);
        }
    }

    #if UNITY_EDITOR
    private static void LogPauseState(PauseState state)
    {
        Debug.Log("Pause status: " + state);

        if (state == PauseState.Paused && Lock.Singleton == null)
        {
            Debug.Log("Locking");
            SceneManager.LoadSceneAsync("Lock", LoadSceneMode.Additive);
        }
    }
    #endif

    public void Set_Input_Fields()
    {
        user.text = User.User_Info.Username;
        password.text = "**********";
        Login_Button.interactable = false;

        if (PlayerPrefs.HasKey("db_key"))
        {
            password_db.text = "**********";
            password_db.interactable = false;
        }
    }

    void Reset_Login_Button()
    {
        login_reponse.color = new Color(1, 1, 1, 0);
    }

    public void Send_Login_Request()
    {
        User.Update_Data(user.text, password.text, true, true);
        Login_Button.interactable = false;
        login_loading.SetActive(true);
    }

    void Handle_Login_Response(string response, Handler_Type type)
    {
        User.Parse_User_Data(response, true, true);
    }

    void Save_DB_Password()
    {
        string key_lock = "aylmvdbsap33ng1q9sh" + SystemInfo.deviceUniqueIdentifier;
        string lock_pwd = Encryption.Decrypt(PlayerPrefs.GetString("lock_pwd"), key_lock);

        string key = "ak3xb7awk8iasdkdHAYakqwr0fqs" + SystemInfo.deviceUniqueIdentifier + lock_pwd;
        string cipher = Encryption.Encrypt(password_db.text, key);
        PlayerPrefs.SetString("db_key", cipher);
        Encryption.Get_DatabaseKey();

        password_db.text = "**********";
        password_db.interactable = false;
    }

    public void On_Load_Success()
    {
        if (password_db.text.Length == 32)
            Save_DB_Password();

        Encryption.Get_DatabaseKey();

        if (!Encryption.Has_Valid_Key())
        {
            Notification_UI_Pop.Show_Message("Llave incorrecta",
                "La llave de la base de datos parece ser incorrecta. Los datos encriptados no podrán ser visualizados.");

            User.User_Info.Role = User.User_Role.default_;
        }

        response = true;
        success = true;
        login_reponse.sprite = Helper.Singleton.Sprite_Login_Success;
    }

    public void On_Load_Failure()
    {
        response = true;
        success = false;
        login_reponse.sprite = Helper.Singleton.Sprite_Login_Failure;
    }

    public void Reset_Input_Fields()
    {
        password.text = "";
    }
}
