// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace AwARe.Notifications.Logic
{
    /// <summary>
    /// Implementation of the abstract notification class for the unity editor environment.
    /// This class is not actively maintained, and only useful for debugging purposes.
    /// </summary>
    public class EditorNotif : Notification
    {
        private DateTime fireTime;
        private string title;
        private string body;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorNotif"/> class.
        /// Notifies the editor's debug output that a new notification has been made.
        /// </summary>
        public EditorNotif()
        {
            Debug.Log("new editor notification created");
        }

        /// <summary>
        /// Sets the time at which this notification is sent.
        /// </summary>
        /// <param name="time">the exact date and time when this notification should be sent.</param>
        public override void SetFireTime(DateTime time)
        {
            fireTime = time;
        }

        /// <summary>
        /// Sets the title text of the notification.
        /// </summary>
        /// <param name="title">the title text to be displayed.</param>
        public override void SetTitle(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// Sets the body text of the notification.
        /// </summary>
        /// <param name="body">The body text to be displayed.</param>
        public override void Setbody(string body)
        {
            this.body = body;
        }

        /// <summary>
        /// Unimplemented. When implemented, sets the questionnaire associated with this notification.
        /// </summary>
        /// <param name="questionnaire">The questionnaire string.</param>
        public override void SetQuestionnaire(string questionnaire)
        {
            Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
        }

        /// <summary>
        /// Schedules the notification to be sent at the time specified in the SetFireTime method.
        /// </summary>
        /// <returns>The data associated with this notification so it can be saved.</returns>
        public override ScheduledNotificationData Schedule()
        {
            Debug.Log("Editor Notification Scheduled: Title: " + title + " Body:" + body + " Scheduled at: " + fireTime.ToString());
            return new ScheduledNotificationData("editor debug id", fireTime.ToString());
        }
    }
}
#endif