// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Attach a toggle to the activity state of its object.
    /// </summary>
    public class ActivityToggle : MonoBehaviour
    {
        /// <summary>
        /// Flip the activity of this GameObject.
        /// </summary>
        public void Toggle() =>
            gameObject.SetActive(!gameObject.activeInHierarchy);

        /// <summary>
        /// Destroy this object. (Permanently disabled)
        /// </summary>
        public void Destroy() =>
            Destroy(gameObject);
    }
}
