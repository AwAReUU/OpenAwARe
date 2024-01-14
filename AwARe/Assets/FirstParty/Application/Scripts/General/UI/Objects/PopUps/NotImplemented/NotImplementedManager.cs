// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Objects;
using AwARe.UI.Popups.Objects;
using UnityEngine;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// A Singleton MonoBehaviour which handles code or other behaviour which has no implementation as of yet.
    /// </summary>
    [RequireComponent(typeof(PopupHandler))]
    public class NotImplementedManager : MonoBehaviour
    {
        // Singleton instance
        private static NotImplementedManager instance;

        // Pop up variables
        private PopupHandler popupHandler;

        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Setup singleton behaviour
            Singleton.Awake(ref instance, this);
            popupHandler = GetComponent<PopupHandler>();

            // Keep alive between scenes
            DontDestroyOnLoad(popupHandler.gameObject);
            DontDestroyOnLoad(popupHandler.supportCanvas);
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
        public static NotImplementedManager Get() =>
            Singleton.Get(ref instance, Instantiate);

        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static NotImplementedManager Instantiate()
        {
            var me = new GameObject("NotImplementedManager");
            me.gameObject.AddComponent<PopupHandler>();
            return me.AddComponent<NotImplementedManager>();
        }

        /// <summary>
        /// Show the Not Implemented popup.
        /// </summary>
        public GameObject ShowPopUp() =>
            popupHandler.ShowPopUp();

        /// <summary>
        /// Hide the Not Implemented popup.
        /// </summary>
        public void HidePopUp() =>
            popupHandler.HidePopUp();
    }
}
