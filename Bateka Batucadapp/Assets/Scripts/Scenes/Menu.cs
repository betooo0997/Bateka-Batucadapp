using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Singleton;

    public enum Menu_item
    {
        None,
        News = 2,
        Polls,
        Events,
        Docs,
        Users,
        Config,
        Users_own,
        News_details,
        Poll_details_other,
        Poll_details_yes_no,
    }

    float alpha_selected;

    [SerializeField]
    float alpha_unselected;

    public Menu_item active_menu_item { get; private set; }
    Menu_item prev_menu_item;

    [SerializeField]
    GameObject home_Button;

    [SerializeField]
    GameObject polls_Button;

    [SerializeField]
    GameObject events_Button;

    [SerializeField]
    GameObject docs_Button;

    [SerializeField]
    GameObject user_Button;

    [SerializeField]
    GameObject config_Button;

    void Awake()
    {
        Singleton = this;
        alpha_unselected = home_Button.GetComponent<Image>().color.a;
    }

    void Start()
    {
        Load_Scene_Menu_Item(Menu_item.News.ToString());
        Message.ShowMessage("¡Hola " + User.User_Info.Username + "!");
    }

    public void Load_Scene_Menu_Item(Menu_item scene)
    {
        Load_Scene_Menu_Item(scene.ToString());
    }

    public void Load_Scene_Menu_Item(string scene_name)
    {
        // Get all Scenes.
        Scene[] scenes = new Scene[SceneManager.sceneCount];

        Enum.TryParse(scene_name, out Menu_item scene);

        if (scene == active_menu_item && active_menu_item != Menu_item.Poll_details_yes_no && active_menu_item != Menu_item.Poll_details_other) return;

        for (int x = 0; x < SceneManager.sceneCount; x++)
            scenes[x] = SceneManager.GetSceneAt(x);

        // Check if scene_name is a Menu_item.
        for (int x = (int)Menu.Menu_item.News; x <= (int)Menu.Menu_item.Poll_details_yes_no; x++)
        {
            if (scene == (Menu.Menu_item)x)
            {
                for (int y = (int)Menu.Menu_item.News; y <= (int)Menu.Menu_item.Poll_details_yes_no; y++)
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

        if (!active_menu_item.ToString().Contains(prev_menu_item.ToString()))
            modify_Buttons(prev_menu_item, change_alpha, alpha_unselected);

        modify_Buttons(active_menu_item, change_alpha, alpha_selected);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, float> method, float new_alpha)
    {
        switch (menu_item)
        {
            case Menu_item.News:
                method(home_Button, new_alpha);
                break;

            case Menu_item.Events:
                method(events_Button, new_alpha);
                break;

            case Menu_item.Polls:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Poll_details_yes_no:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Docs:
                method(docs_Button, new_alpha);
                break;

            case Menu_item.Users:
                method(user_Button, new_alpha);
                break;

            case Menu_item.Users_own:
                method(user_Button, new_alpha);
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
