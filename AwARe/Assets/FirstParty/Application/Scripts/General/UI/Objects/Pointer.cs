// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.InterScenes.Objects;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Handles the pointer UI element. <br/>
    /// The pointer points to the (nearest) surface on camera.
    /// </summary>
    [ExcludeFromCoverage]
    public class Pointer : MonoBehaviour, IPointer
    {
        // GameObject to read.
        [SerializeField] private new Camera camera;

        // Tracking Data
        public Plane lastHitPlane = default;

        /// <summary>
        /// Whether the pointer should be locked on the last found plane.
        /// </summary>
        public bool LockPlane { get; set; } = false;

        /// <summary>
        /// Whether an AR plane has been found.
        /// </summary>
        public bool FoundFirstPlane { get; private set; } = false;

        /// <inheritdoc/>
        public virtual Vector3? PointedAt
        {   get
            {
                // Only return value if it is from a detected plane
                if (!FoundFirstPlane && !Application.isEditor)
                {
                    Debug.LogError("No plane found yet. Please try again.");
                    return null;
                }

                LockPlane = true;
                return transform.position;
            }
        }
            

        protected virtual void Awake()
        {
            camera = camera != null ? camera : ARSecretary.Get().Camera;
            lastHitPlane = new(Vector3.up, camera.transform.position + 1.5f * Vector3.down);

            SetNextPosition();
        }

        protected virtual void Update() =>
            SetNextPosition();

        /// <summary>
        /// Sets the net position of the pointer based on the closest hit ARPlane <br/>
        /// or an intersection of the extended infinite plane of the last ARPlane hit.
        /// </summary>
        [ExcludeFromCoverage]
        private void SetNextPosition()
        {
            // Get a ray shot from the center of the camera.
            Ray ray = camera.ViewportPointToRay(new(0.5f, 0.5f, 0f));

            // Get ARRayCastHits
            var manager = ARSecretary.Get().GetComponent<ARRaycastManager>();
            List<ARRaycastHit> hits = new();
            manager.Raycast(ray, hits, TrackableType.PlaneWithinPolygon);

            // Update transform and last hit plane.
            SetNextPosition(ray, hits.Select(Hit.Create).ToList());
        }

        /// <summary>
        /// Sets the net position of the pointer based on the closest hit ARPlane <br/>
        /// or an intersection of the extended infinite plane of the last ARPlane hit.
        /// </summary>
        /// <param name="ray">A ray cast from.</param>
        /// <param name="hits">AR raycast hits detected.</param>
        public void SetNextPosition(Ray ray, List<Hit> hits)
        {
            // Check if the hit point is on a horizontal ar plane.
            // If so, set pointer to that point.
            if (!LockPlane && HitsARPlane(hits, out Vector3 intersection, out Plane plane))
            {
                FoundFirstPlane = true;
                transform.position = intersection;
                lastHitPlane = plane;
                return;
            }

            // Check if the hit point is TODO
            // If so, set pointer to that point.
            if (HitsLastPlane(ray, lastHitPlane, out intersection))
            {
                transform.position = intersection;
                return;
            }

            // In all other cases, do not change position.
        }

        /// <summary>
        /// Detects the intersection with the ray and the nearest AR Plane, if any.
        /// </summary>
        /// <param name="hits">The results of the AR raycast.</param>
        /// <param name="intersection">The intersection point.</param>
        /// <param name="plane">The (infinite) plane hit.</param>
        /// <returns>True if the ray cast hits.</returns>
        private static bool HitsARPlane(List<Hit> hits, out Vector3 intersection, out Plane plane)
        {
            plane = default;
            intersection = default;

            Hit? hit = ClosestHit(hits);
            if (!hit.HasValue)
                return false;

            intersection = hit.Value.position;
            plane = new(Vector3.up, intersection);
            return true;
        }

        /// <summary>
        /// Gets the closest AR raycast hit among given.
        /// </summary>
        /// <param name="hits">Some AR raycast hits.</param>
        /// <returns>AR raycast hit with shortest distance.</returns>
        private static Hit? ClosestHit(List<Hit> hits)
        {
            if(hits.Count == 0)
                return null;

            Hit closestHit = hits[0];
            float min = closestHit.distance;
            foreach (Hit hit in hits)
                if (hit.distance < min)
                {
                    closestHit = hit;
                    min = hit.distance;
                }

            return closestHit;
        }

        /// <summary>
        /// Checks whether the given ray and plane intersect.
        /// </summary>
        /// <param name="ray">A ray to intersect.</param>
        /// <param name="plane">An infinite plane to intersect.</param>
        /// <param name="intersection">The found intersection point.</param>
        /// <returns>True if they intersect.</returns>
        private static bool HitsLastPlane(Ray ray, Plane plane, out Vector3 intersection)
        {
            bool hit = plane.Raycast(ray, out float distance);
            intersection = ray.origin + distance * ray.direction;
            return hit;
        }

        public struct Hit
        {
            public float distance;
            public Vector3 position;

            public static Hit Create(ARRaycastHit hit) =>
                new(hit.distance, hit.pose.position);

            public Hit(float distance, Vector3 position)
            {
                this.distance = distance;
                this.position = position;
            }
        }
    }
}