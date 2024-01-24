using System;
using System.IO;
using UnityEngine;
using AwARe.Notifications.Logic;
using System.Linq;
using System.Collections.Generic;

namespace AwARe.Notifications.Objects
{
    /// <summary>
    /// Enum that represents the platform the application is running on.
    /// </summary>
    enum Platform
    {
        Android,
        IOS,
        Editor
    }

    /// <summary>
    /// Class <c>NotificationScheduler</c> is responsible for 
    /// </summary>
    public class NotificationScheduler : MonoBehaviour
    {

        /// <summary>
        /// Keep track of which platform the user is running on.
        /// </summary>
        Platform platform;

        /// <summary>
        /// The path in which scheduled notification data are stored.
        /// </summary>
        string folderpath;

        /// <summary>
        /// How many days notifications for this app should be scheduled on.
        /// </summary>
        int scheduleAheadDays;

        /// <summary>
        /// Unity method that is called immediately upon object creation.
        /// Initialises the platform enum variable.
        /// </summary>
        void Awake()
        {
            #if UNITY_EDITOR
                platform = Platform.Editor;
            #elif UNITY_ANDROID
                platform = Platform.Android;
            #elif UNITY_IOS
                platform = Platform.IOS;
            #endif

            folderpath = Path.Combine(Application.persistentDataPath, "Data/ScheduledNotifications");
            scheduleAheadDays = 14;
        }

        /// <summary>
        /// Unity method that is called before the first frame update.
        /// Schedules notifications for the upcoming 2 weeks, unless a notification is already scheduled on a day.
        /// </summary>
        void Start()
        {
            //create the folderpath if it doesn't exist already on the device
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }

            //create fake notification files for if there aren't enough files in the directory
            for (int i = 0; i < scheduleAheadDays; i++)
            {
                string path = Path.Combine(folderpath, "notification" + i);
                if (!File.Exists(path))
                {
                    ScheduledNotificationData fakedata = new ScheduledNotificationData("fakedata", DateTime.Now.AddDays(-1).ToString());
                    Save(fakedata, path);
                }
            }

            //get the filepaths of saved notifications
            string[] filepaths = Directory.GetFiles(folderpath);

            //get the date at which the latest notification is scheduled, 
            //as well as the file paths of any files that store notifications that have passed.
            List<string> unusedFiles = new();
            DateTime latestScheduledNotification = DateTime.MinValue;
            for (int i = 0; i < filepaths.Count(); i++)
            {
                ScheduledNotificationData data = Load(filepaths[i]);

                DateTime scheduledDateTime = DateTime.Parse(data.scheduledTime);

                if (scheduledDateTime < DateTime.Now) unusedFiles.Add(filepaths[i]);
                if (latestScheduledNotification < scheduledDateTime) latestScheduledNotification = scheduledDateTime;
            }
            if (latestScheduledNotification < DateTime.Now) latestScheduledNotification = DateTime.Now;

            //schedule notifications for enough days such that there are notifications scheduled on every day
            //for the number of scheduleAheadDays specified.
            int addDays = (latestScheduledNotification - DateTime.Now).Days + 1;
            int counter = 0;
            foreach (string path in unusedFiles)
            {
                if (counter > scheduleAheadDays)
                {
                    Debug.Log("Somehow there are more files in the saved notifications directory than notifications that should be scheduled");
                    break;
                }

                ScheduledNotificationData data = ScheduleNotification("Daily AwARe Notification",
                "Your daily notification has arrived.", DateTime.Now.AddDays(addDays));

                File.Delete(path);
                Save(data, path);
                addDays++;
                counter++;
            }
        }

        /// <summary>
        /// Saves a ScheduledNotificationData to a file.
        /// </summary>
        /// <param name="data">The data instance to save.</param>
        /// <param name="filepath">The full path to the file (including name to give it).</param>
        public void Save(ScheduledNotificationData data, string filepath)
        {
            // get the data path of this save data
            string dataPath = filepath;

            string jsonData = JsonUtility.ToJson(data, true);

            // create the file in the path if it doesn't exist
            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(dataPath));
            }

            // attempt to save here data
            try
            {
                // save datahere
                File.WriteAllText(dataPath, jsonData);
                Debug.Log("Save data to: " + dataPath);
            }
            catch (Exception e)
            {
                // write out error here
                Debug.LogError("Failed to save data to: " + dataPath);
                Debug.LogError("Error " + e.Message);
            }
        }

        /// <summary>
        /// Loads a ScheduledNotificationData from a file.
        /// </summary>
        /// <param name="filename">The name of the file to load.</param>
        /// <returns>The ScheduledNotificationData instance.</returns>
        public ScheduledNotificationData Load(string filename)
        {
            // get the data path of this save data
            string dataPath = Path.Combine(folderpath, filename);

            // if the file path or name does not exist, return the default SO
            if (!Directory.Exists(Path.GetDirectoryName(dataPath)))
            {
                Debug.LogWarning("File or path does not exist! " + dataPath);
                return null;
            }

            // load in the save data as byte array
            string jsonData = null;

            try
            {
                jsonData = File.ReadAllText(dataPath);
                Debug.Log("Loading data from: " + dataPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to load data from: " + dataPath);
                Debug.LogWarning("Error: " + e.Message);
                return null;
            }

            if (jsonData == null)
                return null;

            ScheduledNotificationData returnedData = JsonUtility.FromJson<ScheduledNotificationData>(jsonData);
            return returnedData;
        }

        /// <summary>
        /// Creates and Schedules a notification.
        /// </summary>
        /// <param name="title">The title of the notification.</param>
        /// <param name="body">The body text of the notification.</param>
        /// <param name="dateTime">The date and time at which to display the notification.</param>
        /// <returns>The ScheduledNotificationData instance (for saving purposes).</returns>
        private ScheduledNotificationData ScheduleNotification(string title, string body, DateTime dateTime)
        {
            ScheduledNotificationData data = null;
            switch(platform)
            {
                case Platform.Android:
                    data = ScheduleAndroidNotification(title, body, dateTime);
                    break;
                case Platform.IOS:
                    data = ScheduleIOSNotification(title, body, dateTime);
                    break;
                case Platform.Editor:
                    data = ScheduleEditorNotification(title, body, dateTime);
                    break;
                default:
                    Debug.Log("No platform detected");
                    break;
            }
            return data;
        }

        /// <summary>
        /// Sends a notifcation to the android platform. does nothing if called on other platforms.
        /// </summary>
        /// <param name="title">The title text of the notification.</param>
        /// <param name="body">The body text of the notification.</param>
        /// <param name="time">The time at which to send the notification.</param>
        private ScheduledNotificationData ScheduleAndroidNotification(string title, string body, DateTime time)
        {
            ScheduledNotificationData data = null;
            #if UNITY_ANDROID
            Notification notification = new AndroidNotif();
            SetNotifParams(notification, title, body, time);
            data = notification.Schedule();
            #endif
            return data;
        }

        /// <summary>
        /// Sends a notifcation to the IOS platform. does nothing if called on other platforms.
        /// </summary>
        /// <param name="title">The title text of the notification.</param>
        /// <param name="body">The body text of the notification.</param>
        /// <param name="time">The time at which to send the notification.</param>
        private ScheduledNotificationData ScheduleIOSNotification(string title, string body, DateTime time)
        {
            ScheduledNotificationData data = null;
            #if UNITY_IOS
            Notification notification = new IOSNotif();
            SetNotifParams(notification, title, body, time);
            data = notification.Schedule();
            #endif
            return data;
        }

        /// <summary>
        /// Sends a notifcation to the unity editor environment. does nothing if called on other platforms.
        /// </summary>
        /// <param name="title">The title text of the notification.</param>
        /// <param name="body">The body text of the notification.</param>
        /// <param name="time">The time at which to send the notification.</param>
        private ScheduledNotificationData ScheduleEditorNotification(string title, string body, DateTime time)
        {
            ScheduledNotificationData data = null;
            #if UNITY_EDITOR
            Notification notification = new EditorNotif();
            SetNotifParams(notification, title, body, time);
            data = notification.Schedule();
            #endif
            return data;
        }

        /// <summary>
        /// Sets the parameters of a notification.
        /// </summary>
        /// <param name="notification">The instance of the implementation of the Notification (ios, android or editor). </param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="body">The body of the notification.</param>
        /// <param name="time">The time at which the notification will be sent.</param>
        private void SetNotifParams(Notification notification, string title, string body, DateTime time)
        {
            notification.SetFireTime(time);
            notification.SetTitle(title);
            notification.Setbody(body);
        }
    }
}