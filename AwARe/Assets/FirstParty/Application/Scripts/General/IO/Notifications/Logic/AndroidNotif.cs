#if UNITY_ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;


namespace AwARe.Notifications.Logic
{
    /// <summary>
    /// Implementation of the abstract notification class for the android platform
    /// </summary>
    public class AndroidNotif : Notification
    {
        private static AndroidNotificationChannel channel;
        private static bool hasChannel = false;

        private AndroidNotification notification;

        /// <summary>
        /// class constructor. initialises some variables necessairy for sending notifications
        /// on the android platform. 
        /// </summary>
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

        /// <summary>
        /// sets the time at which this notification is sent
        /// </summary>
        /// <param name="time">the exact date and time when this notification should be sent</param>
        public override void SetFireTime(DateTime time)
        {
            notification.FireTime = time;
        }

        /// <summary>
        /// sets the title text of the notification
        /// </summary>
        /// <param name="title">the title text to be displayed</param>
        public override void SetTitle(string title)
        {
            notification.Title = title;
        }

        /// <summary>
        /// sets the body text of the notification
        /// </summary>
        /// <param name="body">the body text to be displayed</param>
        public override void Setbody(string body)
        {
            notification.Text = body;
        }

        /// <summary>
        /// unimplemented. When implemented, sets the questionnaire associated with this notification
        /// </summary>
        /// <param name="questionnaire">the questionnaire string</param>
        public override void SetQuestionnaire(string questionnaire)
        {
            //implementation will probably use notification.intentdata
            Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
        }

        /// <summary>
        /// schedules the notification to be sent at the time specified in the SetFireTime method
        /// </summary>
        public override void Send()
        {
            AndroidNotificationCenter.SendNotification(notification, "channel");
        }
    }
}
#endif