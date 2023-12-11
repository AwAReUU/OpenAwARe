// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A polygon representing (a part of) the floor.
    /// </summary>
    public class Polygon
    {
        public List<Vector3> Points;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">A list of pre-defined points of which the polygon consists.</param>
        public Polygon(List<Vector3> points = null)
        {
            if (points == null)
                Points = new List<Vector3>();
            else
                Points = points;
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

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static Polygon FromJson(string json)
        {
            return JsonUtility.FromJson<Polygon>(json);
        }
    }
}
