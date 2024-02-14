// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.UI
{
    public interface IPointer
    {
        /// <summary>
        /// Gets the current position pointed to by this object, if it points at anything.
        /// </summary>
        /// <value>
        /// The current position pointed to by this object.
        /// </value>
        public Vector3? PointedAt { get; }

        /// <summary>
        /// Whether the pointer should be locked on a plane.
        /// </summary>
        public bool LockPlane { get; set; }

        /// <summary>
        /// Whether an AR plane has been found.
        /// </summary>
        public bool FoundFirstPlane { get; }
    }
}