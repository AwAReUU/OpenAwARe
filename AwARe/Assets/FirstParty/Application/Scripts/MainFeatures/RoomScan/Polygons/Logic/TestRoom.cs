// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

using AwARe.Data.Logic;

using UnityEngine;

namespace AwARe.RoomScan.Polygons.Logic
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
            List<Vector3> positivePoints = new()
            {
                new Vector3(-5,-1.5f,-5),
                new Vector3(5,-1.5f,-4),
                new Vector3(5, -1.5f, 6),
                new Vector3(-4,-1.5f,6)
            };

            List<Vector3> negativePoints1 = new()
            {
                new Vector3(0,-1.5f,0),
                new Vector3(1,-1.5f,0),
                new Vector3(1,-1.5f,2),
                new Vector3(0,-1.5f,2)
            };

            List<Vector3> negativePoints2 = new()
            {
                new Vector3(-3,-1.5f,-3),
                new Vector3(-2,-1.5f,-3),
                new Vector3(-2,-1.5f,-1),
                new Vector3(-2.3f,-1.5f,-1.2f),
                new Vector3(-2.8f,-1.5f,-1.3f)
            };

            PositivePolygon = CreatePolygon(positivePoints);
            NegativePolygons = new()
            {
                CreatePolygon(negativePoints1),
                CreatePolygon(negativePoints2)
            };
        }

        /// <summary>
        /// Creates a Polygon and fills its points list with the given points.
        /// </summary>
        /// <param name="points">The points of which the Polygon consists.</param>
        /// <returns>The Polygon consisting of the given points.</returns>
        Polygon CreatePolygon(List<Vector3> points)
        {
            Polygon polygon = new();
            foreach (Vector3 point in points)
            {
                polygon.points.Add(point);
            }
            return polygon;
        }
    }
}
