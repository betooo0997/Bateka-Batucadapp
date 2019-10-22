using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public enum Menu_item
    {
        None,
        Home = 2,
        User,
        Events,
        Polls,
        Config
    }

    float alpha_selected;

    [SerializeField]
    float alpha_unselected;

    Menu_item active_menu_item;
    Menu_item prev_menu_item;

    [SerializeField]
    GameObject home_Button;

    [SerializeField]
    GameObject user_Button;

    [SerializeField]
    GameObject events_Button;

    [SerializeField]
    GameObject polls_Button;

    [SerializeField]
    GameObject config_Button;

    void Start()
    {
        alpha_unselected = home_Button.GetComponent<Image>().color.a;
        Load_Scene_Menu_Item(Menu_item.Home.ToString());
        Message.ShowMessage("Bienvenid@");
    }

    public void Load_Scene_Menu_Item(string scene_name)
    {
        // Get all Scenes.
        Scene[] scenes = new Scene[SceneManager.sceneCount];

        Enum.TryParse(scene_name, out Menu_item scene);

        if (scene == active_menu_item) return;

        for (int x = 0; x < SceneManager.sceneCount; x++)
            scenes[x] = SceneManager.GetSceneAt(x);

        // Check if scene_name is a Menu_item.
        for (int x = (int)Menu.Menu_item.Home; x <= (int)Menu.Menu_item.Config; x++)
        {
            if (scene == (Menu.Menu_item)x)
            {
                for (int y = (int)Menu.Menu_item.Home; y <= (int)Menu.Menu_item.Config; y++)
                {
                    if (y == x) continue;

                    for (int z = 0; z < scenes.Length; z++)
                        if (y == scenes[z].buildIndex)
                            SceneManager.UnloadSceneAsync(y);
                }
                break;
            }
        }

        SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Additive);
        prev_menu_item = active_menu_item;
        active_menu_item = scene;

        modify_Buttons(active_menu_item, change_alpha, alpha_selected);
        modify_Buttons(prev_menu_item, change_alpha, alpha_unselected);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, float> method, float new_alpha)
    {
        switch (menu_item)
        {
            case Menu_item.Home:
                method(home_Button, new_alpha);
                break;

            case Menu_item.User:
                method(user_Button, new_alpha);
                break;

            case Menu_item.Events:
                method(events_Button, new_alpha);
                break;

            case Menu_item.Polls:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Config:
                method(config_Button, new_alpha);
                break;
        }
    }

    void change_alpha(GameObject game_object, float new_alpha)
    {
        Image image = game_object.GetComponent<Image>();
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, new_alpha);
    }
}
