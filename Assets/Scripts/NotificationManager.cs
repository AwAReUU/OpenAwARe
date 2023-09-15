using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications;
using Unity.Notifications.Android;
using UnityEngine.Android;

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

            if(channel.Id == "channel") gameObject.GetComponent<Image>().color = Color.blue;
        #endif

        //IOS code
        #if UNITY_IOS

            //todo


        #endif
    }
}