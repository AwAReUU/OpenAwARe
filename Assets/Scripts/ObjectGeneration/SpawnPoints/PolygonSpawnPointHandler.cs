using System.Collections.Generic;
using UnityEngine;
using RoomScan;

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
        public PolygonSpawnPointHandler(float spacing = 0.1f)
        {
            gridSpacing = spacing;
        }

        private readonly float gridSpacing;

        /// <summary>
        /// Call the parameterized method "GetGridPoints" to obtain spawnPoints.
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetValidSpawnPoints(Room room) => GetGridPoints(room, gridSpacing);

        /// <summary>
        /// Create a 2d bounding box around the polygon points.
        /// </summary>
        /// <param name="polygon">The polygon to obtain the bounding box of.</param>
        /// <returns>Bounding box of the polygon</returns>
        private Bounds CalculateBounds(Polygon polygon)
        {
            List<Vector3> points = polygon.GetPointsList();
            Bounds bounds = new Bounds(points[0], Vector3.zero);
            foreach (var point in points)
            {
                bounds.Encapsulate(point);
            }
            return bounds;
        }

        /// <summary>
        /// Create a grid of points on the plane.
        /// </summary>
        /// <param name="polygon">Polygon to get spawnPoints from.</param>
        /// <param name="spacing">Distance between spawnPoints.</param>
        /// <returns>List of spawnPoints.</returns>
        private List<Vector3> GetGridPoints(Room room, float spacing)
        {
            List<Vector3> result = new();

            Polygon posPolygon = room.PositivePolygon;

            // Calculate the bounds of the polygon
            Bounds bounds = CalculateBounds(posPolygon);

            // Define the height of the polygon
            float y = posPolygon.GetPoints()[0].y;

            // Get all points in bounding box in grid pattern with spacing "spacing" in between
            for (float x = bounds.min.x; x <= bounds.max.x; x += spacing)
            {
                for (float z = bounds.min.z; z <= bounds.max.z; z += spacing)
                {
                    Vector3 gridPoint = new Vector3(x, y, z);

                    // Check if the grid point is inside the polygon
                    if (PolygonHelper.IsPointInsidePolygon(posPolygon, gridPoint) && PolygonHelper.PointNotInPolygons(room.NegativePolygons, gridPoint))
                        result.Add(gridPoint);
                }
            }
            return result;
        }
    }
}