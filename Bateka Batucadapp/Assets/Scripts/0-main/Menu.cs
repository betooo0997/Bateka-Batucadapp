#pragma warning disable 0649

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
        Poll_details_single,
        Poll_details_multi,
        Rhythms,
        Send_Notification,
        Edit,
        Construction
    }

    public GameObject Button_Home, Button_News, Button_Polls, Button_Events, Button_Media;

    public Sprite Sprite_News_Unread, Sprite_Polls_Unread, Sprite_Events_Unread, Sprite_Media_Unread;

    public Sprite Sprite_News_Unread_Sel, Sprite_Polls_Unread_Sel, Sprite_Events_Unread_Sel, Sprite_Media_Unread_Sel;

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

        if (scene == Active_Item && Active_Item != Menu_item.Poll_details_single && Active_Item != Menu_item.Events_details) return;

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
            modify_Buttons(Prev_Item, change_sprite, false);

        modify_Buttons(Active_Item, change_sprite, true);

        SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, Sprite, Sprite, Sprite, Sprite, bool, bool> method, bool selected)
    {
        switch (menu_item)
        {
            case Menu_item.Home:
                Title_Handler.Singleton.Set_Title();
                method(Button_Home, sprite_selected_home, sprite_unselected_home, null, null, selected, false);
                break;

            case Menu_item.News:
                Title_Handler.Singleton.Set_Title("Noticias");
                method(Button_News, sprite_selected_news, sprite_unselected_news, Sprite_News_Unread_Sel, Sprite_News_Unread, selected, Database_Handler.Unread[typeof(News)]);
                break;

            case Menu_item.News_details:
                Title_Handler.Singleton.Set_Title("Noticias", () => Load_Scene_Menu_Item(Menu_item.News));
                method(Button_News, sprite_selected_news, sprite_unselected_news, Sprite_News_Unread_Sel, Sprite_News_Unread, selected, Database_Handler.Unread[typeof(News)]);
                break;

            case Menu_item.Events:
                Title_Handler.Singleton.Set_Title("Eventos");
                method(Button_Events, sprite_selected_events, sprite_unselected_events, Sprite_Events_Unread_Sel, Sprite_Events_Unread, selected, Database_Handler.Unread[typeof(Calendar_Events)]);
                break;

            case Menu_item.Events_details:
                Title_Handler.Singleton.Set_Title("Eventos", () => Load_Scene_Menu_Item(Menu_item.Events));
                method(Button_Events, sprite_selected_events, sprite_unselected_events, Sprite_Events_Unread_Sel, Sprite_Events_Unread, selected, Database_Handler.Unread[typeof(Calendar_Events)]);
                break;

            case Menu_item.Polls:
                Title_Handler.Singleton.Set_Title("Encuestas");
                method(Button_Polls, sprite_selected_polls, sprite_unselected_polls, Sprite_Polls_Unread_Sel, Sprite_Polls_Unread, selected, Database_Handler.Unread[typeof(Polls)]);
                break;

            case Menu_item.Poll_details_single:
                Title_Handler.Singleton.Set_Title("Encuestas", () => Load_Scene_Menu_Item(Menu_item.Polls));
                method(Button_Polls, sprite_selected_polls, sprite_unselected_polls, Sprite_Polls_Unread_Sel, Sprite_Polls_Unread, selected, Database_Handler.Unread[typeof(Polls)]);
                break;

            case Menu_item.Media:
                Title_Handler.Singleton.Set_Title("Otros");
                method(Button_Media, sprite_selected_media, sprite_unselected_media, Sprite_Media_Unread_Sel, Sprite_Media_Unread, selected, false);
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

            case Menu_item.Send_Notification:
                Title_Handler.Singleton.Set_Title("Notificaciones", () => Load_Scene_Menu_Item(Menu_item.Users));
                break;

            case Menu_item.Edit:
                Title_Handler.Singleton.Set_Title("Editor", () => Load_Scene_Menu_Item(Prev_Item));
                break;

            case Menu_item.Construction:
                Title_Handler.Singleton.Set_Title("Hide");
                break;
        }
    }

    void change_sprite(GameObject game_object, Sprite sprite_selected, Sprite sprite_unselected, Sprite sprite_selected_unread, Sprite sprite_unselected_unread, bool selected, bool unread = false)
    {
        Image image = game_object.GetComponentInChildren<Image>();

        if (selected)
        {
            if(unread)
                image.sprite = sprite_selected_unread;
            else
                image.sprite = sprite_selected;
        }
        else
        {
            if (unread)
                image.sprite = sprite_unselected_unread;
            else
                image.sprite = sprite_unselected;
        }
    }
}
