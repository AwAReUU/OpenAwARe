using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ObjectGeneration
{
    public class PlaneScanSpawnPointHandler : ISpawnPointHandler
    {
        private readonly float gridSpacing;
        private readonly ARPlaneManager planeManager;
        public PlaneScanSpawnPointHandler(ARPlaneManager manager, float spacing = 0.1f)
        {
            gridSpacing = spacing;
            planeManager = manager;
        }

        /// <summary>
        /// Get a list of planes where objects are allowed to be spawned on.
        /// </summary>
        /// <param name="planeManager"></param>
        /// <returns></returns>
        private List<ARPlane> GetSpawnPlanes()
        {
            List<ARPlane> validPlanes = new List<ARPlane>();
            foreach (var plane in planeManager.trackables)
            {
                if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.Vertical)
                    continue; //skip walls
                if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalDown)
                    continue; //skip ceilings
                validPlanes.Add(plane);
            }
            return validPlanes;
        }

        public List<Vector3> GetValidSpawnPoints()
        {
            List<Vector3> spawnPoints = new List<Vector3>();
            foreach (var plane in GetSpawnPlanes())
                spawnPoints.AddRange(GetGridPoints(plane, gridSpacing));

            return spawnPoints;
        }

        /// <summary>
        /// Divides a plane into a grid, and obtains all points on the intersections
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="spacing"></param>
        /// <returns></returns>
        private List<Vector3> GetGridPoints(ARPlane plane, float spacing)
        {
            List<Vector3> result = new List<Vector3>();

            Bounds bounds = plane.gameObject.GetComponent<Renderer>().bounds;
            float y = plane.transform.position.y;

            //get all pnts in bounding box in grid pattern with space "spacing" in between.
            for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
            {
                for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
                {
                    Vector3 gridPoint = new Vector3(x, y, z);

                    //The ray origin is the pnt, but moved slightely up.
                    Vector3 rayOrigin = gridPoint + Vector3.up;

                    //cast a ray to the plane. check if it hits.
                    Ray ray = new Ray(rayOrigin, -plane.normal);
                    RaycastHit hit;

                    //if it hits the plane, we know that the gridpoint is on top of the plane.
                    if (Physics.Raycast(ray, out hit, 5))
                        if (hit.collider.gameObject == plane.gameObject)
                            result.Add(gridPoint); 
                }
            }
            return result;
        }
    }
}
