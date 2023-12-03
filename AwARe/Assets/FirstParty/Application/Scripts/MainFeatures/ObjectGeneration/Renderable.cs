using System.Collections.Generic;

using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// <c>Renderable</c> is a class that contains properties
    /// we need to render a specific gameObject
    /// </summary>
    public class Renderable
    {
        /// <value>
        /// The amount of this renderable to render
        /// </value>
        private int quantity { get; set; }

        /// <value>
        /// The percentage of surface area that this object is allowed to use.
        /// </value>
        public float allowedSurfaceUsage { get; set; } //percentage

        /// <value>
        /// The GameObject prefab to render
        /// </value>
        private GameObject prefab { get; set; }

        /// <value>
        /// Distance from center of object to walls, in all 3 directions.
        /// </value>
        private Vector3 halfExtents { get; set; }

        /// <value>
        /// The scaling to apply to the prefab.
        /// </value>
        private float scaling { get; set; }

        /// <summary>
        /// Construct a new Renderable. Note that we do not fill in the allowedSurfaceUsage yet, since this
        /// is based on the sum of all renderables.
        /// </summary>
        /// <param name="prefab">Prefab to render</param>
        /// <param name="halfExtents">halfExtents of the prefab. Scaling is already applied to this.</param>
        /// <param name="quantity">Quantity of this renderable to be placed.</param>
        /// <param name="scaling">scale of this renderable.</param>
        public Renderable(GameObject prefab, Vector3 halfExtents, int quantity, float scaling)
        {
            this.quantity = quantity;
            //this.allowedSurfaceUsage = only known after all renderables are known
            this.prefab = prefab;
            this.halfExtents = halfExtents;
            this.scaling = scaling;
        }

        /// <summary>
        /// Return the quantity of the current Renderable.
        /// </summary>
        /// <returns>The quantity of the current Renderable.</returns>
        public int GetQuantity() => this.quantity;

        /// <summary>
        /// Return the halfExtents of the current Renderable.
        /// </summary>
        public Vector3 GetHalfExtents() => this.halfExtents;

        /// <summary>
        /// Return the scaling of the current Renderable.
        /// </summary>
        public float GetScaling() => this.scaling;

        /// <summary>
        /// Return the gameobject prefab of the current Renderable.
        /// </summary>
        public GameObject GetPrefab() => this.prefab;

        /// <summary>
        /// For each unique object, find out the percentage of space it will need.
        /// </summary>
        /// <param name="renderables"></param>
        /// <returns>the renderables list with surface ratios added</returns>
        public static List<Renderable> SetSurfaceRatios(List<Renderable> renderables)
        {
            //compute the sum of area of all gameObjects that will be spawned.
            float sumArea = 0;
            for (int i = 0; i < renderables.Count; i++)
            {
                int quantity = renderables[i].quantity;
                Vector3 halfExtents = renderables[i].halfExtents;
                float areaPerClone = halfExtents.x * halfExtents.z * 4;
                float areaClonesSum = areaPerClone * quantity;

                renderables[i].allowedSurfaceUsage = areaClonesSum;
                sumArea += areaClonesSum;
            }

            for (int i = 0; i < renderables.Count; i++)
                renderables[i].allowedSurfaceUsage /= sumArea;

            return renderables;
        }

        /// <summary>
        /// Get all four corners of a <paramref name="renderable"/> 
        /// that is placed at <paramref name="position"/>, so they are global coordinates.
        /// </summary>
        /// <param name="renderable">Renderable to find the corners of.</param>
        /// <param name="position">The location in the global coordinate system.</param>
        /// <returns>the 4 bottom corners of the colliders</returns>
        public static List<Vector3> CalculateColliderCorners(Renderable renderable, Vector3 position)
        {
            // Get the size of the BoxCollider
            Vector3 size = renderable.halfExtents;

            // Calculate the corners
            return new List<Vector3>
            {
                position + new Vector3(-size.x, 0, -size.z),
                position + new Vector3(size.x, 0, -size.z),
                position + new Vector3(-size.x, 0, size.z),
                position + new Vector3(size.x, 0, size.z)
            };
        }

        /// <summary>
        /// The sum of the area of all gameObjects that will be spawned
        /// </summary>
        /// <param name="renderables">All objects to be placed.</param>
        /// <returns>The amount of surface area that all objects are needed in between.</returns>
        float ComputeSpaceNeeded(List<Renderable> renderables)
        {
            //compute the sum of area of all gameobjects that will be spawned.
            float sumArea = 0;
            foreach (var renderable in renderables)
            {
                int quantity = renderable.quantity;
                Vector3 halfExtents = renderable.halfExtents;
                float area = halfExtents.x * halfExtents.z * 4;
                sumArea += area * quantity;
            }
            return sumArea;
        }
    }
}
