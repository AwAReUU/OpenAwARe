#if UNITY_IOS
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.iOS;
using UnityEngine;

namespace AwARe.Notifications.Logic
{
    public class IOSNotif : Notification
    {

        iOSNotification notification = new iOSNotification();

        public IOSNotif()
        {
            notification.ShowInForeground = true;
            notification.ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound);
            notification.Identifier = "default identifier";
            notification.CategoryIdentifier = "default category identifier";
            notification.ThreadIdentifier = "default thread identifier";
        }

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

            Debug.Log("fire time set to " + time.ToString());
        }

        public override void SetTitle(string title)
        {
            notification.Title = title;
        }

        public override void Setbody(string body)
        {
            notification.Body = body;
        }

        public override void SetQuestionnaire(string questionnaire)
        {
            //implementation will probably use notification.data
            Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
        }

        public override void Send()
        {
            iOSNotificationCenter.ScheduleNotification(notification);

            Debug.Log("notification sent/scheduled");
        }
    }
}
#endif