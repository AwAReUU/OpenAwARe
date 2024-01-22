using UnityEngine;

namespace AwARe.UI
{
    public interface IPointer
    {
        /// <summary>
        /// Gets the current position pointed to by this object.
        /// </summary>
        /// <value>
        /// The current position pointed to by this object.
        /// </value>
        public Vector3 PointedAt { get; }

        public bool LockPlane { get; set; }
    }
}