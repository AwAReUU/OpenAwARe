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
    public class Polygon
    {
        /// <summary>
        /// The points representing the polygon.
        /// </summary>
        public List<Vector3> listpoints;

        /// <summary>
        /// Default constructor. Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        public Polygon()
        {
            listpoints = new List<Vector3>();
        }

        /// <summary>
        /// Parameterized constructor. Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points"></param>
        public Polygon(List<Vector3> points)
        {
            listpoints = points ?? new List<Vector3>();
        }

        /// <summary>
        /// Adds the given point to the list of points.
        /// </summary>
        /// <param name="point">The point to add to the list.</param>
        public void AddPoint(Vector3 point) => listpoints.Add(point);

        /// <summary>
        /// Removes the last added point from the list of points.
        /// </summary>
        public void RemoveLastPoint()
        {
            if (listpoints.Count > 0)
            {
                listpoints.RemoveAt(listpoints.Count - 1);
            }
        }

        /// <summary>
        /// Gets the points list in array form.
        /// </summary>
        /// <returns>An array containing the points of the point lsit.</returns>
        public Vector3[] GetPoints() => listpoints.ToArray();

        /// <summary>
        /// Returns the amount of points of the polygon.
        /// </summary>
        /// <returns>The amount of point.</returns>
        public int AmountOfPoints() => listpoints.Count;

        /// <summary>
        /// Gets the first point from the points list.
        /// </summary>
        /// <returns>The first point.</returns>
        public Vector3 GetFirstPoint() => listpoints[0];

        /// <summary>
        /// Whether the polygon is empty.
        /// </summary>
        /// <returns>True if the polygon has no points, false otherwise.</returns>
        public bool IsEmptyPolygon() => listpoints.Count == 0;

        /// <summary>
        /// Gets the surface area of the polygon.
        /// </summary>
        public float Area
        {
            get
            {
                float area = 0f;
                int j = listpoints.Count - 1;

                for (int i = 0; i < listpoints.Count; i++)
                {
                    area += (listpoints[j].x + listpoints[i].x) * (listpoints[j].z - listpoints[i].z);
                    j = i;
                }

                return Math.Abs(area / 2);
            }
        }

    }
}