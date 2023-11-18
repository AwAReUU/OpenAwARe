using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObjectGeneration
{
    class PolygonHelper
    {
        /// <summary>
        /// Get a fake hardcoded polygon for testing purposes.
        /// </summary>
        /// <returns></returns>
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
        /// Point in Polygon algorithm. 
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="point"></param>
        /// <returns></returns>
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
        private static bool IsPointOutsidePolygon(List<Vector3> polygon, Vector3 point) =>
            !IsPointInsidePolygon(polygon, point);

        public static bool ObjectColliderInPolygon(List<Vector3> corners, List<Vector3> polygonPoints)
        => corners.Any(corner => IsPointOutsidePolygon(polygonPoints, corner));
    }
}
