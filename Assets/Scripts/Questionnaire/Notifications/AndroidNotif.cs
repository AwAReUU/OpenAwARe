#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class AndroidNotif : Notification
{
    //private static AndroidNotificationCenter notificationCenter;
    private static AndroidNotificationChannel channel;
    private static bool hasChannel = false;

    private AndroidNotification notification;

    public AndroidNotif()
    {
        //initiate the notification channel if it doesn't exist
        if (!hasChannel)
        {
            //create a new channel with channel ID "channel". the other arguments don't seem to be important
            channel = new AndroidNotificationChannel("channel", "Default channel", "Questionnaire notifications", Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            //get permission to send notifications
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                UnityEngine.Android.Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }
            hasChannel = true;
        }
        notification = new AndroidNotification();

        //To change the icon, it must be changed in the project settings, mobile notifications tab.
        notification.SmallIcon = "smallicon";
        notification.ShowTimestamp = true;
        notification.ShouldAutoCancel = false;
    }

    public override void SetFireTime(DateTime time)
    {
        notification.FireTime = time;
    }

    public override void SetTitle(string title)
    {
        notification.Title = title;
    }

    public override void Setbody(string body)
    {
        notification.Text = body;
    }

    public override void SetQuestionnaire(string questionnaire)
    {
        //implementation will probably use notification.intentdata
        Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
    }

    public override void Send()
    {
        AndroidNotificationCenter.SendNotification(notification, "channel");
    }
}
#endif