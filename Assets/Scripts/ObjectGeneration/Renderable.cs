using UnityEngine;

namespace ObjectGeneration
{
    class Renderable
    {
        public int quantity { get; set; }
        public float allowedSurfaceUsage { get; set; } //percentage
        public GameObject prefab { get; set; }
        public Vector3 halfExtents { get; set; }
        public float scaling { get; set; }
    }
}
