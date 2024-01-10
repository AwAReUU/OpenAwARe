// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.ResourcePipeline.Logic;
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
        public int quantity { get; set; }

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

        public void SetScaling(float newScaling) => this.scaling = newScaling;

        public void SetHalfExtents(Vector3 newHalfExtents) => halfExtents = newHalfExtents;

        /// <value>
        /// The resource type of this renderable.
        /// </value>
        public ResourceType resourceType { get; set; }

        /// <value>
        /// The current percentage of surface area used by this renderable.
        /// </value>
        public float currentRatioUsage { get; set; }

        /// <value>
        /// Dictionary containing locations of instances of this renderable & the stacked height at that point. 
        /// </value>
        public Dictionary<Vector3, float> objStacks { get; set; }

        /// <summary>
        /// Construct a new Renderable. Note that we do not fill in the allowedSurfaceUsage yet, since this
        /// is based on the sum of all renderables.
        /// </summary>
        /// <param name="prefab">Prefab to render</param>
        /// <param name="halfExtents">halfExtents of the prefab. Scaling is already applied to this.</param>
        /// <param name="quantity">Quantity of this renderable to be placed.</param>
        /// <param name="scaling">scale of this renderable.</param>
        /// <param name="resourceType">the resource type of this renderable.</param>
        public Renderable(GameObject prefab, Vector3 halfExtents, int quantity, float scaling, ResourceType resourceType)
        {
            this.quantity = quantity;
            //this.allowedSurfaceUsage = only known after all renderables are known
            this.prefab = prefab;
            this.halfExtents = halfExtents;
            this.scaling = scaling;
            this.resourceType = resourceType;
            this.currentRatioUsage = 0;
            this.objStacks = new();
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
        /// Return the resource type of the current Renderable.
        /// </summary>
        public ResourceType GetResourceType() => this.resourceType;

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
        /// The sum of the area of all gameObjects that will be spawned of this renderable
        /// </summary>
        /// <param name="renderables">All objects to be placed.</param>
        /// <returns>The amount of surface area that all objects of this renderabe will need.</returns>
        public float ComputeSpaceNeeded()
        {
            float area = halfExtents.x * halfExtents.z * 4;
            return area * quantity;
        }
    }
}
