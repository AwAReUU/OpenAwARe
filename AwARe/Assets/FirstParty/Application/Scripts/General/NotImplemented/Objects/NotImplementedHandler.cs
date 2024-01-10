using UnityEngine;
using UnityEngine.TestTools;

namespace AwARe.NotImplemented.Objects
{
    [ExcludeFromCoverage]
    public class NotImplementedHandler : MonoBehaviour
    {
        /// <summary>
        /// Show the Not Implemented popup.
        /// </summary>
        public void ShowPopUp() =>
            NotImplementedManager.Get().ShowPopUp();
        
        /// <summary>
        /// Hide the Not Implemented popup.
        /// </summary>
        public void HidePopUp() =>
            NotImplementedManager.Get().HidePopUp();
    }
}
