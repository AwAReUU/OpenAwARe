// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.IO;
using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// Class <c>PolygonSpawnPointHandler</c> is an implementation of <see cref="ISpawnPointHandler"/>
    /// in which a polygon from scanning is used to create spawnPoints on.
    /// </summary>
    public class PolygonSpawnPointHandler : ISpawnPointHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PolygonSpawnPointHandler"/> class.
        /// </summary>
        /// <param name="spacing">Spacing between spawnPoints.</param>
        public PolygonSpawnPointHandler(float spacing = 0.1f)
        {
            gridSpacing = spacing;
        }

        private readonly float gridSpacing;

        /// <summary>
        /// Call the parameterized method "GetGridPoints" to obtain spawnPoints.
        /// </summary>
        /// <param name="room">The room in which the objects will be spawned.</param>
        /// <returns>A list of spawnpoints on which the objects are allowed to be spawned.</returns>
        public List<Vector3> GetValidSpawnPoints(Room room, Mesh path) => GetGridPoints(room, path, gridSpacing);

        /// <summary>
        /// Create a 2d bounding box around the polygon points.
        /// </summary>
        /// <param name="polygon">The polygon to obtain the bounding box of.</param>
        /// <returns>Bounding box of the polygon.</returns>
        private Bounds CalculateBounds(Polygon polygon)
        {
            List<Vector3> points = polygon.Points;
            Bounds bounds = new(points[0], Vector3.zero);
            foreach (var point in points)
            {
                bounds.Encapsulate(point);
            }
            return bounds;
        }


        /// <summary>
        /// Create a grid of points on the plane.
        /// </summary>
        /// <param name="room">Room to get spawnPoints from.</param>
        /// <param name="spacing">Distance between spawnPoints.</param>
        /// <returns>List of spawnPoints.</returns>
        private List<Vector3> GetGridPoints(Room room, Mesh path, float spacing)
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
                    Vector3 gridPoint = new(x, y, z);

                    // Check if the grid point is inside the polygon
                    if (PolygonHelper.IsPointInsidePolygon(posPolygon, gridPoint)
                        && PolygonHelper.PointNotInPolygons(room.NegativePolygons, gridPoint) && !PolygonHelper.IsPointInsidePath(path, gridPoint))
                        result.Add(gridPoint);
                }
            }
            return result;
        }
    }
}