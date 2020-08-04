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
    Notification_User_Searcher searcher;

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

    const string not_permitted_message = "No tienes permiso para poder enviar notificaciones. Contacta con tu administrador/a si quieres obtenerlo.";

    private void Start()
    {
        if (User.User_Info.Role < User.User_Role.moderator)
            Message.ShowMessage(not_permitted_message);
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

            List<string> success = new List<string>();
            List<string> no_device = new List<string>();
            List<string> unknown = new List<string>();

            foreach (User.User_Information info in searcher.Targets)
            {
                Firebase_Handler.Send_Notification(new Firebase_Handler.FCM_Params()
                {
                    User_Id = info.Id,
                    Title = title.text,
                    Body = body.text,
                    Data_Pairs = data_pairs,
                    Concluding_Method = (object[] data) =>
                    {
                        if ((string)data[2] == "0")
                            success.Add(info.Username);
                        else if ((string)data[2] == "-1" && (string)data[1] == "notification_key not found")
                            no_device.Add(info.Username);
                        else
                            unknown.Add(info.Username);

                        if(success.Count + no_device.Count + unknown.Count >= searcher.Targets.Count)
                        {
                            string message = "";

                            if (success.Count > 0)
                                message += "Éxito: " + Utils.List_To_String(success, ", ") + ". ";

                            if (no_device.Count > 0)
                                message += "Sin registrar: " + Utils.List_To_String(no_device, ", ") + ". ";

                            if (unknown.Count > 0)
                                message += "Error: " + Utils.List_To_String(unknown, ", ") + ".";

                            Debug.Log(message);
                            Message.ShowMessage(message);
                        }
                    }
                });
            }

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
