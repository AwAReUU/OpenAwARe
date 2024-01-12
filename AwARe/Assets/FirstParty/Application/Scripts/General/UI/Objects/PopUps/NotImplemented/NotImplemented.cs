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
    /// Handles showing and hiding the not implemented popup.
    /// </summary>
    public class NotImplemented : MonoBehaviour
    {
        /// <summary>
        /// Show the Not Implemented popup.
        /// </summary>
        public void ShowPopUp() =>
            NotImplementedHandler.Get().ShowPopUp();
        
        /// <summary>
        /// Hide the Not Implemented popup.
        /// </summary>
        public void HidePopUp() =>
            NotImplementedHandler.Get().HidePopUp();
    }
}