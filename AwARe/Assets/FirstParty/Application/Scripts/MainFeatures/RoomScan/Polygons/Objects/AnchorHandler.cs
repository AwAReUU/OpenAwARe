// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;

namespace AwARe.RoomScan.Polygons.Objects
{
    public class AnchorHandler : MonoBehaviour
    {
        PolygonManager polygonManager;
        Room room;
        [SerializeField] GameObject pointer;
        [SerializeField] GameObject visualAnchorPrefab;

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
        /// Add the added point to the anchors of the polygon.
        /// </summary>
        public void AddAnchor(Vector3 point)
        {
            Instantiate(visualAnchorPrefab, pointer.transform.position, Quaternion.identity);
            room.AddAnchor(point);
            Debug.Log("Count: " + room.Anchors.Count.ToString());
            Debug.Log(room.Anchors.ToString());
        }

        /// <summary>
        /// Move the pointer based on camera aim.
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
    }
}
