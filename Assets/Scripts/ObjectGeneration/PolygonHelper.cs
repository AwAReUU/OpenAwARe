using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ObjectGeneration
{
    /// <summary>
    /// <c>PolygonHelper</c> is a class that contains some helper methods 
    /// for object generation using the polygon method.
    /// </summary>
    public class PolygonHelper
    {
        /// <summary>
        /// Get a fake hardcoded polygon for testing/debugging purposes.
        /// </summary>
        /// <returns>Hardcoded polygon</returns>
        public static List<Vector3> GetMockPolygon()
        {
            return new List<Vector3>() {
                new Vector3(-8f, -0.87f, -8f),
                new Vector3(-8f, -0.87f, -3f),
                new Vector3(-3f, -0.87f, -3f),
                new Vector3(-3f, -0.87f, -8f)
            };
        }

        /// <summary>
        /// Check if the <paramref name="point"/> is inside of the <paramref name="polygon"/>,
        /// By using a Point-in-polygon (even-odd) algorithm.
        /// </summary>
        /// <param name="polygon">The polygon described by points.</param>
        /// <param name="point">The point.</param>
        /// <returns>Whether the <paramref name="point"/> is inside the <paramref name="polygon"/>.</returns>
        private static bool IsPointInsidePolygon(List<Vector3> polygon, Vector3 point)
        {
            bool isInside = false;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector3 pi = polygon[i];
                Vector3 pj = polygon[j];

                if (pi.z < point.z && pj.z >= point.z || pj.z < point.z && pi.z >= point.z)
                    if (pi.x + (point.z - pi.z) / (pj.z - pi.z) * (pj.x - pi.x) < point.x) //crossproduct
                        isInside = !isInside;
                j = i;
            }

            return isInside;
        }

        /// <summary>
        /// Check if all four base <paramref name="corners"/> are inside of the polygon
        /// described by <paramref name="polygonPoints"/>.
        /// </summary>
        /// <param name="corners">Corners of the base of the bounding box of the Object.</param>
        /// <param name="polygonPoints">Points that describe the polygon.</param>
        /// <returns>Whether the object is inside the polygon.</returns>
        public static bool ObjectColliderInPolygon(List<Vector3> corners, List<Vector3> polygonPoints)
            => corners.All(corner => IsPointInsidePolygon(polygonPoints, corner));
    }
}
