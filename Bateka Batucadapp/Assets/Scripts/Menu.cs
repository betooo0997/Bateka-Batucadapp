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
        Home = 2,
        Polls,
        Events,
        Media,
        Users,
        Config,
        Users_details,
        News,
        News_details,
        Docs,
        Docs_details,
        Events_details,
        Poll_details_other,
        Poll_details_yes_no,
        Rhythms,
        Construction
    }

    float alpha_selected;

    [SerializeField]
    float alpha_unselected;

    public static Menu_item Active_Item { get; private set; }
    public static Menu_item Prev_Item { get; private set; }

    [SerializeField]
    GameObject button_home, button_news, button_polls, button_events, button_media;

    [SerializeField]
    Sprite sprite_selected_home, sprite_selected_news, sprite_selected_polls, sprite_selected_events, sprite_selected_media;

    Sprite sprite_unselected_home, sprite_unselected_news, sprite_unselected_polls, sprite_unselected_events, sprite_unselected_media;

    void Awake()
    {
        Singleton = this;
        //alpha_unselected = home_Button.GetComponent<Image>().color.a;

        sprite_unselected_home      = button_home.GetComponentInChildren<Image>().sprite;
        sprite_unselected_news      = button_news.GetComponentInChildren<Image>().sprite;
        sprite_unselected_polls     = button_polls.GetComponentInChildren<Image>().sprite;
        sprite_unselected_events    = button_events.GetComponentInChildren<Image>().sprite;
        sprite_unselected_media      = button_media.GetComponentInChildren<Image>().sprite;
    }

    void Start()
    {
        Load_Scene_Menu_Item(Menu_item.Home.ToString());
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

        if (scene == Active_Item && Active_Item != Menu_item.Poll_details_yes_no && Active_Item != Menu_item.Poll_details_other && Active_Item != Menu_item.Events_details) return;

        for (int x = 0; x < SceneManager.sceneCount; x++)
            scenes[x] = SceneManager.GetSceneAt(x);

        // Check if scene_name is a Menu_item.
        for (int x = (int)Menu.Menu_item.Home; x <= (int)Menu.Menu_item.Construction; x++)
        {
            if (scene == (Menu.Menu_item)x)
            {
                for (int y = (int)Menu.Menu_item.Home; y <= (int)Menu.Menu_item.Construction; y++)
                {
                    if (y == x) continue;

                    for (int z = 0; z < scenes.Length; z++)
                        if (y == scenes[z].buildIndex)
                            SceneManager.UnloadSceneAsync(y);
                }
                break;
            }
        }

        SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
        Prev_Item = Active_Item;
        Active_Item = scene;

        if (!Active_Item.ToString().Contains(Prev_Item.ToString()))
            modify_Buttons(Prev_Item, change_alpha, false);

        modify_Buttons(Active_Item, change_alpha, true);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, Sprite, Sprite, bool> method, bool selected)
    {
        switch (menu_item)
        {
            case Menu_item.Home:
                method(button_home, sprite_selected_home, sprite_unselected_home, selected);
                break;

            case Menu_item.News:
                method(button_news, sprite_selected_news, sprite_unselected_news, selected);
                break;

            case Menu_item.News_details:
                method(button_news, sprite_selected_news, sprite_unselected_news, selected);
                break;

            case Menu_item.Events:
                method(button_events, sprite_selected_events, sprite_unselected_events, selected);
                break;

            case Menu_item.Events_details:
                method(button_events, sprite_selected_events, sprite_unselected_events, selected);
                break;

            case Menu_item.Polls:
                method(button_polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Poll_details_yes_no:
                method(button_polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Poll_details_other:
                method(button_polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Media:
                method(button_media, sprite_selected_media, sprite_unselected_media, selected);
                break;
        }
    }

    void change_alpha(GameObject game_object, Sprite sprite_selected, Sprite sprite_unselected, bool selected)
    {
        Image image = game_object.GetComponentInChildren<Image>();

        if (selected)
            image.sprite = sprite_selected;
        else
            image.sprite = sprite_unselected;
    }
}
