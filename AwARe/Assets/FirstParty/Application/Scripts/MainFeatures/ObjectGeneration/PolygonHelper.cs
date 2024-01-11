// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// <c>PolygonHelper</c> is a class that contains some helper methods 
    /// for object generation using the Polygon method.
    /// </summary>
    public class PolygonHelper
    {
        /// <summary>
        /// Check if the <paramref name="point"/> is inside of the <paramref name="polygon"/>,
        /// by using a Point-in-Polygon (even-odd) algorithm.
        /// </summary>
        /// <param name="polygon">The Polygon described by points.</param>
        /// <param name="point">The point.</param>
        /// <returns>Whether the <paramref name="point"/> is inside the <paramref name="polygon"/>.</returns>
        public static bool IsPointInsidePolygon(Polygon polygon, Vector3 point)
        {
            List<Vector3> polygonPoints = polygon.Points;

            bool isInside = false;
            int j = polygonPoints.Count - 1;

            for (int i = 0; i < polygonPoints.Count; i++)
            {
                Vector3 pi = polygonPoints[i];
                Vector3 pj = polygonPoints[j];

                if (pi.z < point.z && pj.z >= point.z || pj.z < point.z && pi.z >= point.z)
                    if (pi.x + (point.z - pi.z) / (pj.z - pi.z) * (pj.x - pi.x) < point.x) //crossproduct
                        isInside = !isInside;
                j = i;
            }

            return isInside;
        }

        /// <summary>
        /// Check if the point is not inside of any of the given polygons.
        /// </summary>
        /// <param name="polygons">The polygons the point should not be in.</param>
        /// <param name="point">The point to check.</param>
        /// <returns>The polygon in which the point lies. Null if it does not lie in any of the polygons.</returns>
        public static Polygon PointInPolygons(List<Polygon> polygons, Vector3 point)
        {
            foreach (Polygon polygon in polygons)
            {
                if (IsPointInsidePolygon(polygon, point)) return polygon;
            }
            return null;
        }

        /// <summary>
        /// Check if the point lies inside the path.
        /// </summary>
        /// <param name="path">The path Mesh.</param>
        /// <param name="point">The point to do the test on. </param>
        /// <returns>Whether the point lies inside the path. </returns>
        public static bool IsPointInsidePath(Mesh path, Vector3 point)
        {
            // Create Mesh Collider
            var colliderObj = new GameObject("PathCollider");
            var collider = colliderObj.AddComponent<MeshCollider>();
            collider.sharedMesh = path;

            // Cast ray on collider
            bool hit = false;
            if (Physics.Raycast(new Vector3(point.x, 10f, point.z), Vector3.down, out RaycastHit raycastHit))
            {
                hit = raycastHit.transform.name == "PathCollider";
            }

            GameObject.Destroy(colliderObj);

            return hit;
        }

        /// <summary>
        /// Check if all four base <paramref name="corners"/> are inside of the given olygon.
        /// </summary>
        /// <param name="corners">Corners of the base of the bounding box of the Object.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>Whether the object is inside the given polygon.</returns>
        public static bool ObjectColliderAllInPolygon(List<Vector3> corners, Polygon polygon)
            => corners.All(corner => IsPointInsidePolygon(polygon, corner));

        /// <summary>
        /// Check if any of the four base <paramref name="corners"/> is inside of the given olygon.
        /// </summary>
        /// <param name="corners">Corners of the base of the bounding box of the Object.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>Whether any part of the object is inside the given polygon.</returns>
        public static bool ObjectColliderAnyInPolygon(List<Vector3> corners, Polygon polygon)
            => corners.Any(corner => IsPointInsidePolygon(polygon, corner));
    }
}
