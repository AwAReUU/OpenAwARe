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
    /// implementation of the abstract notification class for the unity editor environment
    /// </summary>
    public class EditorNotif : Notification
    {
        private DateTime fireTime;
        private string title;
        private string body;
        private string questionnaire;

        /// <summary>
        /// the constructor. notifies the editor's debug output that a new notification has been made
        /// </summary>
        public EditorNotif()
        {
            Debug.Log("new editor notification created");
        }

        /// <summary>
        /// sets the time at which this notification is sent
        /// </summary>
        /// <param name="time">the exact date and time when this notification should be sent</param>
        public override void SetFireTime(DateTime time)
        {
            fireTime = time;
        }

        /// <summary>
        /// sets the title text of the notification
        /// </summary>
        /// <param name="title">the title text to be displayed</param>
        public override void SetTitle(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// sets the body text of the notification
        /// </summary>
        /// <param name="body">the body text to be displayed</param>
        public override void Setbody(string body)
        {
            this.body = body;
        }

        /// <summary>
        /// unimplemented. When implemented, sets the questionnaire associated with this notification
        /// </summary>
        /// <param name="questionnaire">the questionnaire string</param>
        public override void SetQuestionnaire(string questionnaire)
        {
            Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
        }

        /// <summary>
        /// schedules the notification to be sent at the time specified in the SetFireTime method
        /// </summary>
        public override async void Send()
        {
            Debug.Log("Editor Notification Scheduled: Title: " + title + " Body:" + body + " Scheduled at: " + fireTime.ToString());
            await Task.Delay((int)(fireTime - DateTime.Now).TotalMilliseconds);
            Debug.Log("Editor Notification received!");
        }
    }
}
#endif