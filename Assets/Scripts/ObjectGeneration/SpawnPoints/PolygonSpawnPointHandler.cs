using System.Collections.Generic;
using UnityEngine;

namespace ObjectGeneration
{
    public class PolygonSpawnPointHandler : ISpawnPointHandler
    {
        public PolygonSpawnPointHandler(List<Vector3> polygon, float spacing = 0.1f)
        {
            gridSpacing = spacing;
            polygonPoints = polygon;
        }

        private readonly float gridSpacing;
        private readonly List<Vector3> polygonPoints;

        public List<Vector3> GetValidSpawnPoints() => GetGridPoints(polygonPoints, gridSpacing);

        private Bounds CalculateBounds(List<Vector3> points)
        {
            Bounds bounds = new Bounds(points[0], Vector3.zero);
            foreach (var point in points)
            {
                bounds.Encapsulate(point);
            }
            return bounds;
        }

        private bool IsPointInsidePolygon(List<Vector3> polygon, Vector3 point)
        {
            bool isInside = false;
            int j = polygon.Count - 1;

            for (int i = 0; i < polygon.Count; i++)
            {
                Vector3 pi = polygon[i];
                Vector3 pj = polygon[j];

                if (pi.z < point.z && pj.z >= point.z || pj.z < point.z && pi.z >= point.z)
                    if (pi.x + (point.z - pi.z) / (pj.z - pi.z) * (pj.x - pi.x) < point.x)
                        isInside = !isInside;
                j = i;
            }
            return isInside;
        }
        private List<Vector3> GetGridPoints(List<Vector3> polygon, float spacing)
        {
            List<Vector3> result = new List<Vector3>();

            // Calculate the bounds of the polygon
            Bounds bounds = CalculateBounds(polygon);

            // Define the height of the polygon
            float y = polygon[0].y;

            // Get all points in bounding box in grid pattern with spacing "spacing" in between
            for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
            {
                for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
                {
                    Vector3 gridPoint = new Vector3(x, y, z);

                    // Check if the grid point is inside the polygon
                    if (IsPointInsidePolygon(polygon, gridPoint))
                        result.Add(gridPoint);
                }
            }
            return result;
        }
    }
}

