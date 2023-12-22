using UnityEngine;

namespace AwARe.NotImplemented.Objects
{
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
