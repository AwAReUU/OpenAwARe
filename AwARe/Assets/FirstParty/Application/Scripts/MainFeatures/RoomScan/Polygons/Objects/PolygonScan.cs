// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.RoomScan.Polygons.Logic;
using UnityEngine;

namespace AwARe.RoomScan.Polygons.Objects
{
    /// <summary>
    /// Handles scanning the polygon.
    /// </summary>
    public class PolygonScan : MonoBehaviour
    {
        [SerializeField] private GameObject pointer;
        [SerializeField] private PolygonDrawer polygonDrawer;
        [SerializeField] private GameObject visualAnchorPrefab;
        public bool anchorPhase = false;
        private Room room;

        void Update()
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Physics.Raycast(ray, out RaycastHit hitData);

            if (hitData.transform != null && hitData.transform.gameObject.name.Contains("ARPlane") &&
                    (hitData.normal - Vector3.up).magnitude < 0.05f)
            {
                // Check if hitpoint is on a horizontal ar plane.
                pointer.transform.position = hitData.point;

                polygonDrawer.SetPointer(pointer.transform.position);
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

                        polygonDrawer.SetPointer(pointer.transform.position);
                    }
                }
            }


            if ((UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                || UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (anchorPhase)
                {
                    if (room == null)
                    {
                        PolygonManager polygonManager = FindObjectOfType<PolygonManager>();
                        room = polygonManager.Room;
                    }

                    if (room.Anchors.Count >= 2) return;

                    UnityEngine.GameObject.Instantiate(visualAnchorPrefab, pointer.transform.position, Quaternion.identity);

                    room.AddAnchor(pointer.transform.position);
                }
                else
                    polygonDrawer.AddPoint();
            }
        }

    }
}