using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField]
    Button config_button;

    [SerializeField]
    Button users_button;

    private void Start()
    {
        config_button.onClick.AddListener(() => { Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Config); });
        users_button.onClick.AddListener(() => { Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Users); });
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Loading_Screen.Set_Active(false);
    }
}
