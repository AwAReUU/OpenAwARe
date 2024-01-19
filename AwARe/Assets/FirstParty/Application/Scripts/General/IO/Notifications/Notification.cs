using System;
using System.Collections;
using System.Collections.Generic;
using AwARe.Notifications.Logic;
using UnityEngine;

namespace AwARe.Notifications
{
    /// <summary>
    /// Abstract class from which platform-specific notifications are derived.
    /// Contains several abstract methods which define required behavior.
    /// </summary>
    public abstract class Notification
    {

        /// <summary>
        /// Returns an instance of a notification. 
        /// This is nessecary because you cannot have a method that is both abstract and static.
        /// Used solely for calling the unschedule method.
        /// </summary>
        /// <returns>A platform-specific notification instance.</returns>
        public static Notification Create()
        {
#if UNITY_EDITOR
            return new EditorNotif();
#elif UNITY_ANDROID
            return new AndroidNotif();
#elif UNITY_IOS
            return new IOSNotif();
#endif
        }

        public abstract void SetFireTime(DateTime time);
        public abstract void SetTitle(string title);
        public abstract void Setbody(string body);
        public abstract void SetQuestionnaire(string questionnaire);
        public abstract ScheduledNotificationData Schedule();
        public abstract void Unschedule(ScheduledNotificationData data);
    }

    /// <summary>
    /// Enum that represents the platform the application is running on.
    /// </summary>
    public enum Platform
    {
        Android,
        IOS,
        Editor
    }

    /// <summary>
    /// Class which is used for saving and loading scheduled notifications from persistent memory.
    /// </summary>
    public class ScheduledNotificationData
    {
        public string notificationID;
        public DateTime scheduledTime;
        public bool cancellable;
        public ScheduledNotificationData(string notificationID, DateTime scheduledTime, bool cancellable = true)
        {
            this.notificationID = notificationID;
            this.scheduledTime = scheduledTime;
            this.cancellable = cancellable;
        }
    }
}