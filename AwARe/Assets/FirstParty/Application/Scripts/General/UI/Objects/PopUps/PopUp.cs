using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * To be altered/correct.
 */

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Controls the activation state of a pop-up panel in the graphical user interface.
    /// </summary>
    public class PopUpScript : MonoBehaviour
    {
        [SerializeField] private GameObject popupPanel;

        /// <summary>
        /// Sets the pop-up panel to an active state, making it visible.
        /// </summary>
        public void PopUpOn() =>
            popupPanel.SetActive(true);
        /// <summary>
        /// Sets the pop-up panel to an inactive state, hiding it from view.
        /// </summary>
        public void PopUpOff() =>
            popupPanel.SetActive(false);
    }
}
