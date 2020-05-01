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
        Docs,
        Users,
        Config,
        Users_own,
        News,
        News_details,
        Docs_details,
        Events_details,
        Poll_details_other,
        Poll_details_yes_no,
    }

    float alpha_selected;

    [SerializeField]
    float alpha_unselected;

    public static Menu_item Active_Item { get; private set; }
    public static Menu_item Prev_Item { get; private set; }

    [SerializeField]
    GameObject home_Button;

    [SerializeField]
    GameObject news_Button;

    [SerializeField]
    GameObject polls_Button;

    [SerializeField]
    GameObject events_Button;

    [SerializeField]
    GameObject docs_Button;

    void Awake()
    {
        Singleton = this;
        alpha_unselected = home_Button.GetComponent<Image>().color.a;
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
        for (int x = (int)Menu.Menu_item.Home; x <= (int)Menu.Menu_item.Poll_details_yes_no; x++)
        {
            if (scene == (Menu.Menu_item)x)
            {
                for (int y = (int)Menu.Menu_item.Home; y <= (int)Menu.Menu_item.Poll_details_yes_no; y++)
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
            modify_Buttons(Prev_Item, change_alpha, alpha_unselected);

        modify_Buttons(Active_Item, change_alpha, alpha_selected);
    }

    void modify_Buttons(Menu_item menu_item, Action<GameObject, float> method, float new_alpha)
    {
        switch (menu_item)
        {
            case Menu_item.Home:
                method(home_Button, new_alpha);
                break;

            case Menu_item.News:
                method(news_Button, new_alpha);
                break;

            case Menu_item.News_details:
                method(news_Button, new_alpha);
                break;

            case Menu_item.Events:
                method(events_Button, new_alpha);
                break;

            case Menu_item.Events_details:
                method(events_Button, new_alpha);
                break;

            case Menu_item.Polls:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Poll_details_yes_no:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Poll_details_other:
                method(polls_Button, new_alpha);
                break;

            case Menu_item.Docs:
                method(docs_Button, new_alpha);
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
