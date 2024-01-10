// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Logic;
using TMPro;
using UnityEngine;

namespace AwARe.UI.Popups.Objects
{
    /// <summary>
    /// A Singleton MonoBehaviour which handles thrown errors.
    /// </summary>
    public class ErrorHandler : PopupHandler
    {
        // Singleton instance
        private static ErrorHandler instance;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Setup singleton behaviour
            Singleton.Awake(ref instance, this);

            // Keep alive between scenes
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(supportCanvas);

            Application.logMessageReceived += HandleLog;
        }

        /// <summary>
        /// Called when the behaviour component is destroyed.
        /// </summary>
        private void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        /// <summary>
        /// Get its current instance.
        /// Instantiate a new instance if necessary.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static ErrorHandler Get() =>
            Singleton.Get(ref instance, Instantiate);

        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static ErrorHandler Instantiate()
        {
            GameObject me = new("ErrorHandler");
            me.AddComponent<ErrorHandler>();
            return me.AddComponent<ErrorHandler>();
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type != LogType.Exception && type != LogType.Error) return;
            ShowPopUp();
#if DEBUG
            // Show the specific error
            activePopUp.GetComponentInChildren<TextMeshProUGUI>().text = logString;
#endif
        }
    }
}
