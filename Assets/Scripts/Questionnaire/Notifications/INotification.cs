using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Notification
{
    // protected DateTime fireTime;
    // protected string title;
    // protected string body;
    // protected string questionnaire;

    public abstract void SetFireTime(DateTime time);
    public abstract void SetTitle(string title);
    public abstract void Setbody(string body);
    public abstract void SetQuestionnaire(string questionnaire);
    public abstract void Send();
}