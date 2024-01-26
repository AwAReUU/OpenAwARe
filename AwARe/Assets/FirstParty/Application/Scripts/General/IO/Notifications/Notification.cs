// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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
        /// Abstract method that sets the date and time at which a notification is sent.
        /// </summary>
        /// <param name="time">The date and time at which the notification should be sent.</param>
        public abstract void SetFireTime(DateTime time);
        /// <summary>
        /// Abstract method that sets the title text of a notification.
        /// </summary>
        /// <param name="title">The title text that should be displayed.</param>
        public abstract void SetTitle(string title);
        /// <summary>
        /// Abstract method that sets the body text of a notification.
        /// </summary>
        /// <param name="body">The body text that should be displayed.</param>
        public abstract void Setbody(string body);
        /// <summary>
        /// Abstract method that sets a questionnaire linked to the notification. Not currently implemented.
        /// </summary>
        /// <param name="questionnaire">The string / ID of the questionnaire that should be linked.</param>
        public abstract void SetQuestionnaire(string questionnaire);
        /// <summary>
        /// Abstract method that Schedules the notification.
        /// </summary>
        /// <returns>Data related to the scheduled notifcation.</returns>
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
        /// <summary>
        /// ID of the notification.
        /// </summary>
        public string notificationID;
        /// <summary>
        /// Time at which the notification is sent.
        /// </summary>
        public string scheduledTime;
        /// <summary>
        /// Wether or not it should be possible to cancel the notification.
        /// </summary>
        public bool cancellable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledNotificationData"/> class.
        /// </summary>
        /// <param name="notificationID">The ID of the notification.</param>
        /// <param name="scheduledTime">The DateTime at which the notification will be sent, as a string.</param>
        /// <param name="cancellable">Wether or not the notification can be cancelled.</param>
        public ScheduledNotificationData(string notificationID, string scheduledTime, bool cancellable = true)
        {
            this.notificationID = notificationID;
            this.scheduledTime = scheduledTime;
            this.cancellable = cancellable;
        }
    }
}