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
        public abstract void SetFireTime(DateTime time);
        public abstract void SetTitle(string title);
        public abstract void Setbody(string body);
        public abstract void SetQuestionnaire(string questionnaire);
        public abstract ScheduledNotificationData Schedule();
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
        public string scheduledTime;
        public bool cancellable;
        public ScheduledNotificationData(string notificationID, string scheduledTime, bool cancellable = true)
        {
            this.notificationID = notificationID;
            this.scheduledTime = scheduledTime;
            this.cancellable = cancellable;
        }
    }
}