using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Notifications;
using Unity.Notifications.Android;
using UnityEngine.Android;
public class MarcoButtonScript : MonoBehaviour
{
    // Start is called before the first frame update

    
 
    public void test()
    {
        gameObject.GetComponent<Image>().color = new Color(Random.Range(0 ,1), Random.Range(0 ,1), Random.Range(0 ,1));
    }

    public void testerino()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

        var notification = new AndroidNotification();
        notification.Title = "NOTIFICATION HERE";
        notification.Text = "BINGBONG";
        notification.FireTime = System.DateTime.Now.AddMinutes(1);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");

    }
}
