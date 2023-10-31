using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Platform
{
    Android,
    IOS,
    Editor
}

public class NotificationScheduler : MonoBehaviour
{

    Platform platform;

    //for background scheduling: possibly look into 'service' C# class thing:
    //https://stackoverflow.com/questions/34573109/how-to-make-an-android-app-to-always-run-in-background

    void Awake()
    {
        #if UNITY_EDITOR
        platform = Platform.Editor;
        #elif UNITY_ANDROID
        platform = Platform.Android;
        #elif UNITY_IOS
        platform = Platform.IOS;
        #endif
    }

    //test method, can be transformed into the main method later on.
    public void SendNotificationTest()
    {
        switch(platform)
        {
            case Platform.Android:
                SendAndroidNotification("Test Title", "Test text", "No questionnaire", DateTime.Now);
                break;
            case Platform.IOS:
                SendIOSNotification("Test Title", "Test text", "No questionnaire", DateTime.Now.AddSeconds(1));
                break;
            case Platform.Editor:
                SendEditorNotification("Test Title", "Test text", "No questionnaire", DateTime.Now.AddSeconds(10));
                break;
            default:
                Debug.Log("No platform detected");
                break;
        }
    }

    //platform specific notification sending methods
    private void SendAndroidNotification(string title, string body, string questionnaire, DateTime time)
    {
        #if UNITY_ANDROID
        Notification notification = new AndroidNotif();
        SetNotifParams(notification, title, body, questionnaire, time);
        notification.Send();
        #endif
    }

    private void SendIOSNotification(string title, string body, string questionnaire, DateTime time)
    {
        #if UNITY_IOS
        Notification notification = new IOSNotif();
        SetNotifParams(notification, title, body, questionnaire, time);
        notification.Send();
        #endif
    }

    //debug / unity editor notification
    private void SendEditorNotification(string title, string body, string questionnaire, DateTime time)
    {
        #if UNITY_EDITOR
        Notification notification = new EditorNotif();
        SetNotifParams(notification, title, body, questionnaire, time);
        notification.Send();
        #endif
    }

    private void SetNotifParams(Notification notification, string title, string body, string questionnaire, DateTime time)
    {
        notification.SetFireTime(time);
        notification.SetTitle(title);
        notification.Setbody(body);
        notification.SetQuestionnaire(questionnaire);
    }
}