#pragma warning disable 0649

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Notification_UI_Pop : MonoBehaviour
{
    public static Notification_UI_Pop Singleton;

    static Action action;

    static string action_label;
    static string close_label = "CERRAR";

    [SerializeField]
    Text title_ui, content_ui;

    [SerializeField]
    Button action_button;

    [SerializeField]
    Text action_button_text;

    [SerializeField]
    Text close_button_text;

    static string title;
    static string content;

    void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        title_ui.text = title;
        content_ui.text = content;

        if (action != null)
        {
            action_button.onClick.AddListener(() => { action(); Hide_Message(); });
            action_button_text.text = action_label;
        }

        action_button.gameObject.SetActive(action != null);
        close_button_text.text = close_label;
    }

    public static void Show_Message(string msg_title, string msg_content, Action msg_action = null, string msg_action_label = "", string msg_close_label = "CERRAR")
    {
        title = msg_title.ToUpper();
        content = msg_content;
        action = msg_action;
        action_label = msg_action_label;
        close_label  = msg_close_label.ToUpper();
        Utils.Load_Scene_ST("Notification");
    }

    public void Hide_Message()
    {
        SceneManager.UnloadSceneAsync("Notification");
    }

    public static void Show_Firebase_Message(IDictionary<string, string> data)
    {
        if (data.ContainsKey("Msg_Title"))
            Show_Message(data["Msg_Title"].ToUpper(), data["Msg_Content"]);

        if (data.ContainsKey("Red_Type"))
        {
            Notification_Sender.Redirect_Type type = (Notification_Sender.Redirect_Type)Enum.Parse(typeof(Notification_Sender.Redirect_Type), data["Red_Type"]);
            uint id = uint.Parse(data["Red_Id"]);

            switch (type)
            {
                case Notification_Sender.Redirect_Type.Calendar_Event:
                    Utils.Singleton.StartCoroutine(Redirect(new Calendar_Event() { Id = id }));
                    break;

                case Notification_Sender.Redirect_Type.Poll:
                    Utils.Singleton.StartCoroutine(Redirect(new Poll() { Id = id }));
                    break;

                case Notification_Sender.Redirect_Type.News_Entry:
                    Utils.Singleton.StartCoroutine(Redirect(new News_Entry() { Id = id }));
                    break;
            }
        }
    }

    static System.Collections.IEnumerator Redirect(Data_struct data)
    {
        while (Database_Handler.Data_List_Get(data.Database_Handler_Type()).Count == 0)
            yield return null;

        yield return null;

        List<Data_struct> data_list = Database_Handler.Data_List_Get(data.Database_Handler_Type());
        Data_struct chosen_data = data_list.Find(x => x.Id == data.Id);

        if (!chosen_data.Equals(Activator.CreateInstance(chosen_data.GetType())))
        {
            Database_Handler.Selected_Data = chosen_data;

            switch((Notification_Sender.Redirect_Type)Enum.Parse(typeof(Notification_Sender.Redirect_Type), data.GetType().ToString()))
            {
                case Notification_Sender.Redirect_Type.Calendar_Event:
                    Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Events_details);
                    break;

                case Notification_Sender.Redirect_Type.News_Entry:
                    Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.News_details);
                    break;

                case Notification_Sender.Redirect_Type.Poll:
                    if(((Poll)chosen_data).Votable_Type == Votable_Type.Multiple_Single_Select)
                        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Poll_details_single);
                    else
                        Menu.Singleton.Load_Scene_Menu_Item(Menu.Menu_item.Poll_details_multi);
                    break;
            }
        }
    }
}
