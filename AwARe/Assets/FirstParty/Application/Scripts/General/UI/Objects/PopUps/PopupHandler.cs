// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.UI.Popups.Objects
{
    /// <summary>
    /// A class that handles showing and hiding popups.
    /// </summary>
    public class PopupHandler : MonoBehaviour
    {
        /// <summary>
        /// Prefab of the popup.
        /// </summary>
        public GameObject popUpPrefab;

        /// <summary>
        /// Canvas on which the popup is shown.
        /// </summary>
        public GameObject supportCanvas;

        /// <summary>
        /// The active popup gameObject.
        /// </summary>
        protected GameObject activePopUp;

        /// <summary>
        /// Show the popup.
        /// </summary>
        public GameObject ShowPopUp() =>
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
