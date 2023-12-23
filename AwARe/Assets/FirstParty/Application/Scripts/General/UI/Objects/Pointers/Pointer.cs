// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.InterScenes.Objects;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Handles the pointer UI element. <br/>
    /// The pointer points to the (nearest) surface on camera.
    /// </summary>
    public class Pointer : MonoBehaviour
    {
        // GameObject to read.
        [SerializeField] private new Camera camera;

        // Tracking Data
        [SerializeField] private InfinitePlane lastHitPlane;

        private void Start()
        {
            camera = camera != null ? camera : ARSecretary.Get().Camera;
            lastHitPlane = new(camera.transform.position + 1.5f * Vector3.down, Vector3.up);

            SetNextPosition();
        }

        private void Update()
        {
            SetNextPosition();
        }

        /// <summary>
        /// Sets the net position of the pointer based on the closest hit ARPlane <br/>
        /// or an intersection of the extended infinite plane of the last ARPlane hit.
        /// </summary>
        private void SetNextPosition()
        {
            // Set the ray.
            Ray ray = camera.ViewportPointToRay(new (0.5f, 0.5f, 0f));

            // Check if the hit point is on a horizontal ar plane.
            // If so, set pointer to that point.
            if (HitsHorizontalPlane(ray, out Vector3 pos, out Vector3 normal))
            {
                transform.position = pos;
                lastHitPlane = new(pos, normal);
                return;
            }

            // Check if the hit point is TODO
            // If so, set pointer to that point.
            if (HitsLastPlane(ray, out pos))
            {
                transform.position = pos;
                return;
            }

            // In all other cases, do not change position.
        }

        /// <summary>
        /// Detects the intersection with the ray and the nearest AR Plane, if any.
        /// </summary>
        /// <param name="ray">The ray cast over.</param>
        /// <param name="pos">The intersection point.</param>
        /// <param name="normal">The normal of the plane (Now always up).</param>
        /// <returns>True if the ray cast hits.</returns>
        private bool HitsHorizontalPlane(Ray ray, out Vector3 pos, out Vector3 normal)
        {
            pos = normal = Vector3.zero;

            // Perform ray cast and check if anything hits.
            var manager = ARSecretary.Get().GetComponent<ARRaycastManager>();
            List<ARRaycastHit> hitResults = new();
            if (!manager.Raycast(ray, hitResults
                , TrackableType.PlaneWithinPolygon
                ) || hitResults.Count == 0)
                return false;

            // Take closest hit.
            ARRaycastHit bestHit = hitResults[0];
            float min = bestHit.distance;
            foreach (ARRaycastHit hit in hitResults)
                if (hit.distance < min)
                {
                    bestHit = hit;
                    min = hit.distance;
                }

            pos = bestHit.pose.position;
            normal = Vector3.up;
            return true;
        }

        /// <summary>
        /// Gets the intersection with the ray and the infinite extension of the last AR Plane, if they intersect.
        /// </summary>
        /// <param name="ray">The ray cast over.</param>
        /// <param name="pos">The intersection point.</param>
        /// <returns>True if the ray cast hits.</returns>
        private bool HitsLastPlane(Ray ray, out Vector3 pos)
        {
            pos = Vector3.zero;

            float div = Vector3.Dot(lastHitPlane.Normal, ray.direction);
            if (div == 0)
                return false;

            float num = Vector3.Dot(lastHitPlane.Normal, lastHitPlane.Center - ray.origin);
            float l = num / div;
            if (l <= 0)
                return false;

            pos = l * ray.direction + ray.origin;
            return true;
        }
    }

    /// <summary>
    /// A data type for infinite planes.
    /// </summary>
    public struct InfinitePlane
    {
        /// <summary>
        /// Gets or sets the center point on the infinite plane.
        /// </summary>
        /// <value>
        /// A point on the infinite plane.
        /// </value>
        public Vector3 Center { readonly get => center; set => center = value; }
        [SerializeField] private Vector3 center;
        
        /// <summary>
        /// Gets or sets the normal of the infinite plane.
        /// </summary>
        /// <value>
        /// The normal of the infinite plane.
        /// </value>
        public Vector3 Normal {
            readonly get => normal;
            set
            {
                if (value == Vector3.zero)
                    throw new ArgumentException("The normal may not be zero.");
                normal = value;
            }
        }
        [SerializeField] private Vector3 normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfinitePlane"/> struct.
        /// </summary>
        /// <param name="center">A point on the plane, utilized as center.</param>
        /// <param name="normal">The normal of the plane.</param>
        public InfinitePlane(Vector3 center, Vector3 normal)
        {
            if (normal == Vector3.zero)
                throw new ArgumentException("The normal may not be zero.");

            this.center = center;
            this.normal = normal;
        }
    }
}