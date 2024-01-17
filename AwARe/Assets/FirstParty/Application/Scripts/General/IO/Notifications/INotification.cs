using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.Notifications
{
    /// <summary>
    /// Abstract class from which platform-specific notifications are derived
    /// contains several abstract methods which define required behavior.
    /// </summary>
    public abstract class Notification
    {
        public abstract void SetFireTime(DateTime time);
        public abstract void SetTitle(string title);
        public abstract void Setbody(string body);
        public abstract void SetQuestionnaire(string questionnaire);
        public abstract void Send();
    }
}