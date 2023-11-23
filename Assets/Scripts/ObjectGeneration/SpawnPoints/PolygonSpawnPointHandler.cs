using System.Collections.Generic;
using UnityEngine;

namespace ObjectGeneration
{
    /// <summary>
    /// Class <c>PolygonSpawnPointHandler</c> is an implementation of <see cref="ISpawnPointHandler"/>
    /// in which a polygon from scanning is used to create spawnPoints on.
    /// </summary>
    public class PolygonSpawnPointHandler : ISpawnPointHandler
    {
        /// <summary>
        /// Constructor. Initializes gridSpacing and polygonPoints.
        /// </summary>
        /// <param name="polygon">A polygon described by a list of points.</param>
        /// <param name="spacing">Spacing between spawnPoints.</param>
        public PolygonSpawnPointHandler(List<Vector3> polygon, float spacing = 0.1f)
        {
            gridSpacing = spacing;
            polygonPoints = polygon;
        }

        private readonly float gridSpacing;
        private readonly List<Vector3> polygonPoints;

        /// <summary>
        /// Call the parameterized method "GetGridPoints" to obtain spawnPoints.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetValidSpawnPoints() => GetGridPoints(polygonPoints, gridSpacing);

        /// <summary>
        /// Create a 2d bounding box around the polygon points.
        /// </summary>
        /// <param name="polygon">The polygon to obtain the bounding box of.</param>
        /// <returns>Bounding box of the polygon</returns>
        private Bounds CalculateBounds(List<Vector3> polygon)
        {
            Bounds bounds = new Bounds(polygon[0], Vector3.zero);
            foreach (var point in polygon)
            {
                bounds.Encapsulate(point);
            }
            return bounds;
        }

        /// <summary>
        /// Check if the point lies inside the polygon by using a simple
        /// even/uneven Point-in-polygon algorithm.
        /// </summary>
        /// <param name="polygon">The polygon described by points</param>
        /// <param name="point">The point do the test on.</param>
        /// <returns>Whether the point lies inside the polygon.</returns>
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

        /// <summary>
        /// Create a grid of points on the plane.
        /// </summary>
        /// <param name="polygon">Polygon to get spawnPoints from.</param>
        /// <param name="spacing">Distance between spawnPoints.</param>
        /// <returns>List of spawnPoints.</returns>
        private List<Vector3> GetGridPoints(List<Vector3> polygon, float spacing)
        {
            List<Vector3> result = new();

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

