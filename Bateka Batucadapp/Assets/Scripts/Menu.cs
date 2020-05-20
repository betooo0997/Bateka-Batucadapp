using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Menu Singleton;

    public static Menu_item Active_Item { get; private set; }
    public static Menu_item Prev_Item { get; private set; }

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

    public GameObject Button_Home, Button_News, Button_Polls, Button_Events, Button_Media;

    public Sprite Sprite_News_Red, Sprite_Polls_Red, Sprite_Events_Red, Sprite_Media_Red;

    [SerializeField]
    Sprite sprite_selected_home, sprite_selected_news, sprite_selected_polls, sprite_selected_events, sprite_selected_media;

    [SerializeField]
    float alpha_unselected;
    float alpha_selected;

    Sprite sprite_unselected_home, sprite_unselected_news, sprite_unselected_polls, sprite_unselected_events, sprite_unselected_media;

    void Awake()
    {
        Singleton = this;
        //alpha_unselected = home_Button.GetComponent<Image>().color.a;

        sprite_unselected_home      = Button_Home.GetComponentInChildren<Image>().sprite;
        sprite_unselected_news      = Button_News.GetComponentInChildren<Image>().sprite;
        sprite_unselected_polls     = Button_Polls.GetComponentInChildren<Image>().sprite;
        sprite_unselected_events    = Button_Events.GetComponentInChildren<Image>().sprite;
        sprite_unselected_media      = Button_Media.GetComponentInChildren<Image>().sprite;
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

        Prev_Item = Active_Item;
        Active_Item = scene;

        if (!Active_Item.ToString().Contains(Prev_Item.ToString()))
            modify_Buttons(Prev_Item, change_alpha, false);

        modify_Buttons(Active_Item, change_alpha, true);

        SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, Sprite, Sprite, bool> method, bool selected)
    {
        switch (menu_item)
        {
            case Menu_item.Home:
                Title_Handler.Singleton.Set_Title();
                method(Button_Home, sprite_selected_home, sprite_unselected_home, selected);
                break;

            case Menu_item.News:
                Title_Handler.Singleton.Set_Title("Noticias");
                method(Button_News, sprite_selected_news, sprite_unselected_news, selected);
                break;

            case Menu_item.News_details:
                Title_Handler.Singleton.Set_Title("Noticias", () => Load_Scene_Menu_Item(Menu_item.News));
                method(Button_News, sprite_selected_news, sprite_unselected_news, selected);
                break;

            case Menu_item.Events:
                Title_Handler.Singleton.Set_Title("Eventos");
                method(Button_Events, sprite_selected_events, sprite_unselected_events, selected);
                break;

            case Menu_item.Events_details:
                Title_Handler.Singleton.Set_Title("Eventos", () => Load_Scene_Menu_Item(Menu_item.Events));
                method(Button_Events, sprite_selected_events, sprite_unselected_events, selected);
                break;

            case Menu_item.Polls:
                Title_Handler.Singleton.Set_Title("Encuestas");
                method(Button_Polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Poll_details_yes_no:
                Title_Handler.Singleton.Set_Title("Encuestas", () => Load_Scene_Menu_Item(Menu_item.Polls));
                method(Button_Polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Poll_details_other:
                Title_Handler.Singleton.Set_Title("Encuestas", () => Load_Scene_Menu_Item(Menu_item.Polls));
                method(Button_Polls, sprite_selected_polls, sprite_unselected_polls, selected);
                break;

            case Menu_item.Media:
                Title_Handler.Singleton.Set_Title("Otros");
                method(Button_Media, sprite_selected_media, sprite_unselected_media, selected);
                break;

            case Menu_item.Users:
                Title_Handler.Singleton.Set_Title("Usuarios", () => Load_Scene_Menu_Item(Menu_item.Home));
                break;

            case Menu_item.Users_details:
                Title_Handler.Singleton.Set_Title("Usuarios", () => Load_Scene_Menu_Item(Menu_item.Users));
                break;

            case Menu_item.Rhythms:
                Title_Handler.Singleton.Set_Title("Ritmos", () => Load_Scene_Menu_Item(Menu_item.Media));
                break;

            case Menu_item.Config:
                Title_Handler.Singleton.Set_Title("Opciones", () => Load_Scene_Menu_Item(Menu_item.Home));
                break;

            case Menu_item.Construction:
                Title_Handler.Singleton.Set_Title("Hide");
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
