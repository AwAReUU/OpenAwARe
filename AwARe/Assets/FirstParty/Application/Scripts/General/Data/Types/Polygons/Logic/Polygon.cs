// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A polygon representing (a part of) the floor.
    /// </summary>
    /// 
    public class Polygon

    {
        /// <summary>
        /// The points of the polygon.
        /// </summary>
        public List<Vector3> Points;

        // Default constructor
        public Polygon()
        {
            Points = new List<Vector3>();
        }
        
        // Parameterized constructor
        public Polygon(List<Vector3> points)
        {
            Points = points ?? new List<Vector3>();
        }

        /// <summary>
        /// Adds the given point to the list of points.
        /// </summary>
        /// <param name="point">The point to add to the list.</param>
        public void AddPoint(Vector3 point) => Points.Add(point);

        /// <summary>
        /// Removes the last added point from the list of points.
        /// </summary>
        public void RemoveLastPoint()
        {
            if (Points.Count > 0)
            {
                Points.RemoveAt(Points.Count - 1);
            }
        }

        /// <summary>
        /// Gets the points list in array form.
        /// </summary>
        /// <returns>An array containing the points of the point lsit.</returns>
        public Vector3[] GetPoints() => Points.ToArray();

        /// <summary>
        /// Returns the amount of points of the polygon.
        /// </summary>
        /// <returns>The amount of point.</returns>
        public int AmountOfPoints() => Points.Count;

        /// <summary>
        /// Gets the first point from the points list.
        /// </summary>
        /// <returns>The first point.</returns>
        public Vector3 GetFirstPoint() => Points[0];

        /// <summary>
        /// Whether the polygon is empty.
        /// </summary>
        /// <returns>True if the polygon has no points, false otherwise.</returns>
        public bool IsEmptyPolygon() => Points.Count == 0;

        /// <summary>
        /// Calculates the area of the polygon.
        /// </summary>
        /// <returns>The area of the polygon.</returns>
        public float Area
        {
            get
            {
                float area = 0f;
                int j = Points.Count - 1;

                for (int i = 0; i < Points.Count; i++)
                {
                    area += (Points[j].x + Points[i].z) * (Points[j].z - Points[i].z);
                    j = i;
                }

                return Math.Abs(area / 2);
            }
        }

    }
}
