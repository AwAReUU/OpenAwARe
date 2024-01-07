// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;
using Codice.CM.Common.Merge;

using UnityEngine;

namespace AwARe.RoomScan.Polygons.Objects
{
    public class AnchorHandler : MonoBehaviour
    {
        private PolygonManager polygonManager;
        private Room room;
        [SerializeField] private GameObject pointer;
        [SerializeField] private GameObject visualAnchorPrefab;
        private Vector3 anchor1, anchor2;
        private float anchorDist, anchorAlpha;

        private void Awake()
        {
            polygonManager = FindObjectOfType<PolygonManager>();
            room = polygonManager.Room;
        }

        private void Update()
        {
            MovePointer();

            if ((UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                || UnityEngine.Input.GetMouseButtonDown(0))
            {
                AddAnchor(pointer.transform.position);
            }
        }

        /// <summary>
        /// Move the pointer based on camera aim. The pointer is always centered.
        /// </summary>
        private void MovePointer()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Physics.Raycast(ray, out RaycastHit hitData);

            if (hitData.transform != null && hitData.transform.gameObject.name.Contains("ARPlane") &&
                    (hitData.normal - Vector3.up).magnitude < 0.05f)
            {
                // Check if hitpoint is on a horizontal ar plane.
                pointer.transform.position = hitData.point;
            }
            else
            {
                // Check if plane and ray are not parrallel.
                if (ray.direction.y != 0)
                {
                    float l = (-1.5f - ray.origin.y) / ray.direction.y;
                    // Check if ray is not reversed
                    if (l > 0f)
                    {
                        pointer.transform.position = ray.origin + ray.direction * l;
                    }
                }
            }
        }

        /// <summary>
        /// Add the added point to the anchors of the polygon.
        /// </summary>
        public void AddAnchor(Vector3 point)
        {
            if (room.Anchors.Count >= 2) return;

            Instantiate(visualAnchorPrefab, pointer.transform.position, Quaternion.identity);
            room.AddAnchor(point);
            // Debug.Log("Count: " + room.Anchors.Count.ToString());
            // Debug.Log(room.Anchors.ToString());
        }

        /// <summary>
        /// Initialize anchor data references.
        /// </summary>
        private void FindAnchorData()
        {
            anchor1 = room.Anchors[0];
            anchor2 = room.Anchors[1];
            anchorDist = GetVector2Distance(anchor1, anchor2);
            anchorAlpha = GetVector2Alpha(anchor1, anchor2);
        }

        private float GetVector2Distance(Vector3 a, Vector3 b)
        {
            float dX = b.x - a.x;
            float dY = b.y - a.y;
            float r = Mathf.Sqrt(dX * dX + dY * dY);

            return r;
        }

        private float GetVector2Alpha(Vector3 a, Vector3 b)
        {
            float dX = b.x - a.x;
            float dY = b.y - a.y;
            float alpha = Mathf.Atan(dY / dX);

            if (dX < 0 && dY >= 0)
                return alpha + Mathf.PI;
            if (dX < 0 && dY < 0)
                return alpha - Mathf.PI;

            //if (x >= 0 && y >= 0)
            //|| (x >= 0 && y < 0)
            return alpha;
        }

        private Vector3 WorldToPolar(Vector3 worldPoint)
        {
            float r = GetVector2Distance(anchor2, worldPoint);
            float alpha = GetVector2Alpha(anchor2, worldPoint);

            Vector3 polarPoint = new(r, alpha, worldPoint.z);
            return polarPoint;
        }

        private Vector3 PolarToWorld(Vector3 polarPoint)
        {
            float r = polarPoint.x;
            float alpha = polarPoint.y;

            float wx = r * Mathf.Cos(alpha + anchorAlpha);
            float wy = r * Mathf.Sin(alpha + anchorAlpha);
            Vector3 worldPoint = new(wx, wy, polarPoint.z);

            return anchor2 + worldPoint;
        }
    }
}
