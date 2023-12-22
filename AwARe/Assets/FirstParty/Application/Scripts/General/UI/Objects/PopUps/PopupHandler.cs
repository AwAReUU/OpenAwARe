// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Logic;

using UnityEngine;

namespace AwARe.NotImplemented.Objects
{
    /// <summary>
    /// A Singleton MonoBehaviour which handles code or other behaviour which has no implementation as of yet.
    /// </summary>
    public class PopupHandler : MonoBehaviour
    {
        // Not implemented Prefabs and canvas
        [SerializeField] protected GameObject popUpPrefab;
        [SerializeField] protected GameObject supportCanvas;

        // Active GameObjects
        protected GameObject activePopUp;
        
        /// <summary>
        /// Show the popup.
        /// </summary>
        public void ShowPopUp() =>
            activePopUp = activePopUp != null ? activePopUp : Instantiate(popUpPrefab, supportCanvas.transform);
        
        /// <summary>
        /// Hide the popup.
        /// </summary>
        public void HidePopUp()
        {
            Destroy(activePopUp);
            activePopUp = null;
        }
    }
}
