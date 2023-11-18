using UnityEngine;

namespace ObjectGeneration
{
    class Renderable
    {
        /// <value>
        /// The amount of this renderable to render
        /// </value>
        public int quantity { get; set; }
        /// <value>
        /// The percentage of surface area that this object is allowed to use.
        /// </value>
        public float allowedSurfaceUsage { get; set; } //percentage
        /// <value>
        /// The GameObject prefab to render
        /// </value>
        public GameObject prefab { get; set; }
        /// <value>
        /// Distance from center of object to walls, in all 3 directions.
        /// </value>
        public Vector3 halfExtents { get; set; }
        /// <value>
        /// The scaling to apply to the prefab.
        /// </value>
        public float scaling { get; set; }
    }
}
