// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Objects;

using UnityEngine;

namespace AwARe.NotImplemented.Objects
{
    /// <summary>
    /// A Singleton MonoBehaviour which handles code or other behaviour which has no implementation as of yet.
    /// </summary>
    public class NotImplementedManager : MonoBehaviour
    {
        // Singleton instance
        private static NotImplementedManager instance;

        // Not implemented Prefabs and canvas
        [SerializeField] private GameObject popUpPrefab;
        [SerializeField] private GameObject supportCanvas;

        // Active GameObjects
        private GameObject activePopUp;

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
            GameObject me = new("NotImplementedManager");
            return me.AddComponent<NotImplementedManager>();
        }

        /// <summary>
        /// Show the Not Implemented popup.
        /// </summary>
        public GameObject ShowPopUp()
        {
            activePopUp = activePopUp != null ? activePopUp : Instantiate(popUpPrefab, supportCanvas.transform);
            return activePopUp;
        }

        /// <summary>
        /// Hide the Not Implemented popup.
        /// </summary>
        public void HidePopUp()
        {
            Destroy(activePopUp);
            activePopUp = null;
        }
    }
}
