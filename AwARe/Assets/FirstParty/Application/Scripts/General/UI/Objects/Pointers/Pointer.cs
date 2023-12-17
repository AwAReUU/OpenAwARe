// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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

        #if UNITY_EDITOR
        // Monitor fields for development
        [SerializeField] private Vector3 lastHitPosition;
        [SerializeField] private Vector3 lastHitNormal;
        #endif

        // Data members
        private (Vector3, Vector3) lastHitPlane;

        private void Start()
        {
            camera = camera != null ? camera : ARSecretary.Get().Camera;
            lastHitPlane = (camera.transform.position + 1.5f * Vector3.down, Vector3.up);
            (lastHitPosition, lastHitNormal) = lastHitPlane;
            SetNextPosition();
        }

        private void Update()
        {
            SetNextPosition();
            (lastHitPosition, lastHitNormal) = lastHitPlane;
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
                lastHitPlane = (pos, normal);
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

            (Vector3 center, Vector3 normal) = lastHitPlane;
            float div = Vector3.Dot(normal, ray.direction);
            if (div == 0)
                return false;

            float num = Vector3.Dot(normal, center - ray.origin);
            float l = num / div;
            if (l <= 0)
                return false;

            pos = l * ray.direction + ray.origin;
            return true;
        }




        //private void SetNextPosition()
        //{
        //    // Perform ray cast.
        //    Ray ray = camera.ViewportPointToRay(new (0.5f, 0.5f, 0f));
            
        //    // Check if the hit point is on a horizontal ar plane.
        //    // If so, set pointer to that point.
        //    if (HitsHorizontalPlane(ray, out Vector3 pos, out Vector3 normal))
        //    {
        //        transform.position = lastHitPosition = pos;
        //        lastHitNormal = normal;
        //        return;
        //    }
            
        //    // Check if the hit point is TODO
        //    float l = 0;
        //    bool hitsLastPlane = ray.direction.y != 0
        //        && (l = (-1.5f - ray.origin.y) / ray.direction.y) > 0f;

        //    // If so, set pointer to that point.
        //    if (hitsLastPlane)
        //    {
        //        transform.position = ray.origin + ray.direction * l;
        //    }

        //    // In all other cases, do not change position.
        //}

        //private bool HitsHorizontalPlane(Ray ray, out Vector3 pos, out Vector3 normal)
        //{
        //    // Perform ray cast.
        //    Physics.Raycast(ray, out RaycastHit hit);
            
        //    // Get data from the hit.
        //    pos = hit.point;
        //    normal = hit.normal;

        //    // Check if the hit point is on a horizontal ar plane.
        //    return hit.transform != null
        //        && hit.transform.gameObject.name.Contains("ARPlane")
        //        && (normal - Vector3.up).magnitude < 0.05f;


        //}

        //private bool HitsLastPlane(Ray ray, out Vector3 pos)
        //{
        //    pos = Vector3.zero;
        //    if (ray.direction.y == 0)
        //        return false;

        //    float l = (-1.5f - ray.origin.y) / ray.direction.y;
        //    if (l <= 0f)
        //        return false;

        //    pos = ray.origin + ray.direction * l;
        //    return true;
        //}
    }
}