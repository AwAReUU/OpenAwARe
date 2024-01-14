// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;
using UnityEngine.TestTools;

namespace AwARe.UI.Objects
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
