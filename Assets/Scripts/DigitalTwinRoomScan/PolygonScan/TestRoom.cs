//*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;

namespace RoomScan
{
    /// <summary>
    /// A room filled with pre-defined polygons.
    /// </summary>
    public class TestRoom : Room
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRoom"/> class.
        /// </summary>
        public TestRoom()
        {
            List<Vector3> posPoints = new()
            {
                new Vector3(-5,-1.5f,-5),
                new Vector3(5,-1.5f,-4),
                new Vector3(5, -1.5f, 6),
                new Vector3(-4,-1.5f,6)
            };

            List<Vector3> neg1Points = new()
            {
                new Vector3(0,-1.5f,0),
                new Vector3(1,-1.5f,0),
                new Vector3(1,-1.5f,2),
                new Vector3(0,-1.5f,2)
            };

            List<Vector3> neg2Points = new()
            {
                new Vector3(-3,-1.5f,-3),
                new Vector3(-2,-1.5f,-3),
                new Vector3(-2,-1.5f,-1),
                new Vector3(-2.3f,-1.5f,-1.2f),
                new Vector3(-2.8f,-1.5f,-1.3f)
            };

            PositivePolygon = CreatePolygon(posPoints);
            NegativePolygons = new()
            {
                CreatePolygon(neg1Points),
                CreatePolygon(neg2Points)
            };
        }

        /// <summary>
        /// Creates a polygon and fills its points list with the given points
        /// </summary>
        /// <param name="points">The points of which the polygon consists</param>
        /// <returns>The polygon consisting of the given points</returns>
        Polygon CreatePolygon(List<Vector3> points)
        {
            Polygon polygon = new();
            foreach (Vector3 point in points)
            {
                polygon.AddPoint(point);
            }
            return polygon;
        }
    }
}
