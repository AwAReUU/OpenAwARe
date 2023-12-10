// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Objects
{
    /// <summary>
    /// Destroy GameObject when running.
    /// </summary>
    public class EditorDummy : MonoBehaviour
    {
        /// <summary>
        /// Only for editing purposes. Destroy immediately on activation.
        /// </summary>
        protected void Awake() 
        {
            Destroy(gameObject);
        }
    }
}