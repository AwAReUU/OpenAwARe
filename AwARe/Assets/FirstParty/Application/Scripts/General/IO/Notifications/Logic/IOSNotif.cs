#if UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.iOS;
using UnityEngine;

namespace AwARe.Notifications.Logic
{
    /// <summary>
    /// Implementation of the abstract notification class for the IOS platform.
    /// </summary>
    public class IOSNotif : Notification
    {

        iOSNotification notification = new iOSNotification();
        DateTime fireTime;

         /// <summary>
        /// Class constructor. initialises some variables necessairy for sending notifications
        /// on the IOS platform. 
        /// </summary>
        public IOSNotif()
        {
            notification.ShowInForeground = true;
            notification.ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound);
            notification.Identifier = "default identifier";
            notification.CategoryIdentifier = "default category identifier";
            notification.ThreadIdentifier = "default thread identifier";
        }

        /// <summary>
        /// Sets the time at which this notification is sent.
        /// </summary>
        /// <param name="time">The exact date and time when this notification should be sent.</param>
        public override void SetFireTime(DateTime time)
        {
            var trigger = new iOSNotificationCalendarTrigger()
            {
                Year = time.Year,
                Month = time.Month,
                Day = time.Day,
                Hour = time.Hour,
                Minute = time.Minute,
                Second = time.Second,
                Repeats = false
            };
            notification.Trigger = trigger;
            fireTime = time;
            Debug.Log("fire time set to " + time.ToString());
        }

        /// <summary>
        /// Sets the title text of the notification.
        /// </summary>
        /// <param name="title">The title text to be displayed.</param>
        public override void SetTitle(string title)
        {
            notification.Title = title;
        }

        /// <summary>
        /// Sets the body text of the notification.
        /// </summary>
        /// <param name="body">The body text to be displayed.</param>
        public override void Setbody(string body)
        {
            notification.Body = body;
        }

        /// <summary>
        /// Unimplemented. When implemented, sets the questionnaire associated with this notification.
        /// </summary>
        /// <param name="questionnaire">The questionnaire string.</param>
        public override void SetQuestionnaire(string questionnaire)
        {
            //implementation will probably use notification.data
            Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
        }

        /// <summary>
        /// Schedules the notification to be sent at the time specified in the SetFireTime method.
        /// </summary>
        public override ScheduledNotificationData Schedule()
        {
            iOSNotificationCenter.ScheduleNotification(notification);
            return new ScheduledNotificationData(notification.Identifier, fireTime);
        }

        public override void Unschedule(ScheduledNotificationData data)
        {
            iOSNotificationCenter.RemoveScheduledNotification(data.notificationID);
        }
    }
}
#endif