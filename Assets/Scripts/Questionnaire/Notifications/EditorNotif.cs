#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class EditorNotif : Notification
{

    private DateTime fireTime;
    private string title;
    private string body;
    private string questionnaire;

    public EditorNotif()
    {
        Debug.Log("new editor notification created");
    }

    public override void SetFireTime(DateTime time)
    {
        fireTime = time;
    }

    public override void SetTitle(string title)
    {
        this.title = title;
    }

    public override void Setbody(string body)
    {
        this.body = body;
    }

    public override void SetQuestionnaire(string questionnaire)
    {
        Debug.Log("SetQuestionnaire is used to make the notification open a questionnaire when it is tapped. Not currently implemented");
    }

    public override async void Send()
    {
        Debug.Log("Editor Notification Scheduled: Title: " + title + " Body:" + body + " Scheduled at: " + fireTime.ToString());
        await Task.Delay((int)(fireTime - DateTime.Now).TotalMilliseconds);
        Debug.Log("Editor Notification received!");
    }
}
#endif