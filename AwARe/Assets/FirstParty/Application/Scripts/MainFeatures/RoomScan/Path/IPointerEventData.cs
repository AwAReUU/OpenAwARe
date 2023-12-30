// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine.EventSystems;

namespace AwARe
{
    /// <summary>
    /// An interface to access recorded pointer event data.
    /// </summary>
    public interface IPointerEventData
    {
        /// <summary>
        /// Gets the last recorded pointer event data, including the on screen position on the last pointer event.
        /// </summary>
        /// <value>
        /// The last recorded pointer event data.
        /// </value>
        public PointerEventData PointerEventData { get; }
    }
}