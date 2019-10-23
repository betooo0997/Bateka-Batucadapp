using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class Notifications
{
    public static void NewNotification(string title, string content, System.DateTime time, bool large_icon = true)
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();
        notification.Style = NotificationStyle.BigTextStyle;
        notification.Title = title;
        notification.Text = content;
        notification.FireTime = time;
        notification.SmallIcon = "icon_0";
        if (large_icon) notification.LargeIcon = "icon_0";

        var identifier = AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
