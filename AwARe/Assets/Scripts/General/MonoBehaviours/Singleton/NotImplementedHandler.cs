// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using AwARe.MonoBehaviours;
using UnityEngine;

namespace AwARe.MonoBehaviours
{
    public class NotImplementedHandler : MonoBehaviour
    {
        // Singleton instance
        private static NotImplementedHandler instance;
        // Not implemented Prefabs and canvas
        [SerializeField] private GameObject popUpPrefab;
        [SerializeField] private GameObject supportCanvas;

        // Active GameObjects
        private GameObject activePopUp;

        private void Awake()
        {
            // Setup singleton behaviour
            Singleton.Awake(ref instance, this);
            // Keep alive between scenes
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(supportCanvas);
        }

        private void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);
    
        /// <summary>
        /// Get its current instance.
        /// Instantiate a new instance if necessary.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static NotImplementedHandler Get() =>
            Singleton.Get(ref instance, Instantiate);

        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static NotImplementedHandler Instantiate()
        {
            GameObject me = new("NotImplementedHandler");
            me.AddComponent<NotImplementedHandler>();
            return me.AddComponent<NotImplementedHandler>();
        }

        public void ShowPopUp() =>
            activePopUp = activePopUp != null ? activePopUp : Instantiate(popUpPrefab, supportCanvas.transform);

        public void HidePopUp()
        {
            Destroy(activePopUp);
            activePopUp = null;
        }
    }
}
