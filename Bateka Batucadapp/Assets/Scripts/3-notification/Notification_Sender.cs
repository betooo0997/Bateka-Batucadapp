#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification_Sender : MonoBehaviour
{
    public enum Message_Type
    {
        No_Message,
        Copy_Notification,
        Custom
    }

    public enum Redirect_Type
    {
        No_Redirect,
        Calendar_Event,
        Poll,
        News_Entry
    }

    [SerializeField]
    Dropdown user_dropdwon;

    [SerializeField]
    InputField title, body;

    [SerializeField]
    InputField title_message, body_message;

    [SerializeField]
    InputField redirect_id;

    [SerializeField]
    GameObject confirm_parent;

    Message_Type message_type;

    Redirect_Type redirect_type;

    List<string> options;
    Dictionary<string, uint> user_ids;

    uint user_id;

    const string not_permitted_message = "No tienes permiso para poder enviar notificaciones. Contacta con tu administrador/a si quieres obtenerlo.";

    private void Start()
    {
        options = new List<string>();
        user_ids = new Dictionary<string, uint>();

        foreach (User.User_Information user in User.Users_Info)
        {
            options.Add(user.Name);
            user_ids.Add(user.Name, user.Id);
        }

        options.Add(User.User_Info.Name);
        user_ids.Add(User.User_Info.Name, User.User_Info.Id);

        user_dropdwon.AddOptions(options);

        if (User.User_Info.Role < User.User_Role.moderator)
            Message.ShowMessage(not_permitted_message);
    }

    public void On_User_Change(int value)
    {
        user_id = user_ids[options[value]];
    }

    public void On_Message_Type_Change(int value)
    {
        message_type = (Message_Type)value;
        title_message.transform.parent.gameObject.SetActive(value > 1);
        Utils.InvokeNextFrame(() => {
            gameObject.SetActive(false);
            Utils.InvokeNextFrame(() => {
                gameObject.SetActive(true);
            });
        });
    }

    public void On_Redirect_Type_Change(int value)
    {
        redirect_type = (Redirect_Type)value;
        redirect_id.transform.parent.gameObject.SetActive(value > 0);
        Utils.InvokeNextFrame(() => {
            gameObject.SetActive(false);
            Utils.InvokeNextFrame(() => {
                gameObject.SetActive(true);
            });
        });
    }

    public void Send_Notification()
    {
        if(User.User_Info.Role < User.User_Role.moderator)
            Message.ShowMessage(not_permitted_message);
        else if (title.text.Length <= 5)
            Message.ShowMessage("Debes especificar un título");
        else if (body.text.Length <= 5)
            Message.ShowMessage("Debes especificar el contenido");
        else
            confirm_parent.SetActive(true);
    }

    public void Confirm_Send_Notification()
    {
        confirm_parent.SetActive(false);

        if (User.User_Info.Role >= User.User_Role.moderator)
        {
            Dictionary<string, string> data_pairs = new Dictionary<string, string>();

            switch (message_type)
            {
                case Message_Type.Copy_Notification:
                    data_pairs.Add("Msg_Title", title.text);
                    data_pairs.Add("Msg_Content", body.text);
                    break;

                case Message_Type.Custom:
                    data_pairs.Add("Msg_Title", title_message.text);
                    data_pairs.Add("Msg_Content", body_message.text);
                    break;
            }

            if (redirect_type != Redirect_Type.No_Redirect)
            {
                data_pairs.Add("Red_Type", redirect_type.ToString());
                data_pairs.Add("Red_Id", redirect_id.text);
            }

            Firebase_Handler.Send_Notification(new Firebase_Handler.FCM_Params()
            {
                User_Id = user_id,
                Title = title.text,
                Body = body.text,
                Data_Pairs = data_pairs,
                Concluding_Method = (object[] data) => 
                {
                    if ((string)data[2] == "0")
                        Message.ShowMessage("Notificación enviada con éxito");
                    else if ((string)data[2] == "-1" && (string)data[1] == "notification_key not found")
                        Message.ShowMessage("Error: ningún dispositivo registrado bajo este usuario.");
                    else
                        Message.ShowMessage("Error: error desconocido.");
                }
            });

            title.text = "";
            body.text = "";
            title_message.text = "";
            body_message.text = "";
            redirect_id.text = "";
        }
        else
            Message.ShowMessage(not_permitted_message);
    }
}
