using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications;
using Unity.Notifications.Android;
using UnityEngine.Android;
//using Unity.Notifications.iOS;
//using UnityEngine.iOS;

public class NotificationManager : MonoBehaviour
{
    AndroidNotificationChannel channel;

    // Start is called before the first frame update
    void Start()
    {
        //Android code
        #if UNITY_ANDROID
            //create a new channel with channel ID "channel". the other arguments don't seem to be important
            channel = new AndroidNotificationChannel("channel", "Default channel", "Questionnaire notifications", Importance.Default);
            AndroidNotificationCenter.RegisterNotificationChannel(channel);

            //get permission to send notifications
            if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
            }

        #endif

        //IOS code
        #if UNITY_IOS

        #endif
    }

    // Update is called once per frame
    void Update()
    {
        //Android code
        #if UNITY_ANDROID

        #endif

        //IOS code
        #if UNITY_IOS

        #endif
    }


    public void SendNotification(int timefromnow)
    {
        //Android code
        #if UNITY_ANDROID
            AndroidNotification notification = new AndroidNotification();
            notification.Title = "Questionnaire Ready";
            notification.Text = "A New Questionnaire is waiting to be completed";
            notification.SmallIcon = "smallicon";
            notification.LargeIcon = "largeicon";
            notification.ShowTimestamp = true;
            notification.FireTime = System.DateTime.Now.AddSeconds(timefromnow);
            notification.ShouldAutoCancel = true;

            AndroidNotificationCenter.SendNotification(notification, "channel");

            //todo: use notificationstringdata to direct the notification to the correct page once it has been implemented
            //see https://docs.unity3d.com/Packages/com.unity.mobile.notifications@2.2/manual/Android.html 'Store and retrieve custom data'

        #endif

        //IOS code
        #if UNITY_IOS

        //example notification template from the documentation
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new TimeSpan(0, 0, timefromnow),
            Repeats = false
        };

        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = "_notification_01",
            Title = "Title",
            Body = "Scheduled at: " + DateTime.Now.ToShortDateString() + " triggered in 5 seconds",
            Subtitle = "This is a subtitle, something, something important...",
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = "category_a",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger,
        };

        // iOSNotificationCenter.ScheduleNotification(notification);

        #endif
    }
}