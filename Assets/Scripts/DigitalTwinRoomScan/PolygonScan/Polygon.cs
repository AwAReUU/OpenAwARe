// /*                                                                                       *\
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
    /// A polygon representing (a part of) the floor.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        /// The points that form the 'corners' of the polygon.
        /// </summary>
        protected List<Vector3> points;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">A list of pre-defined points of which the polygon consists.</param>
        public Polygon(List<Vector3> points = null)
        {
            if (points == null)
                this.points = new List<Vector3>();
            else
                this.points = points;
        }

        /// <summary>
        /// Adds the given point to the list of points.
        /// </summary>
        /// <param name="point">The point to add to the list.</param>
        public void AddPoint(Vector3 point)
        {
            points.Add(point);
        }

        /// <summary>
        /// Removes the last added point from the list of points.
        /// </summary>
        public void RemoveLastPoint()
        {
            if (points.Count > 0)
            {
                this.points.RemoveAt(this.points.Count - 1);
            }
        }

        /// <summary>
        /// Gets the points list in array form.
        /// </summary>
        /// <returns>An array containing the points of the point lsit.</returns>
        public Vector3[] GetPoints()
        {
            return this.points.ToArray();
        }

        /// <summary>
        /// Gets the points list.
        /// </summary>
        /// <returns>The points list.</returns>
        public List<Vector3> GetPointsList()
        {
            return this.points;
        }

        /// <summary>
        /// Returns the amount of points of the polygon.
        /// </summary>
        /// <returns>The amount of point.</returns>
        public int AmountOfPoints()
        {
            return points.Count;
        }

        /// <summary>
        /// Gets the first point from the points list.
        /// </summary>
        /// <returns>The first point.</returns>
        public Vector3 GetFirstPoint()
        {
            return points[0];
        }

        /// <summary>
        /// Whether the polygon is empty.
        /// </summary>
        /// <returns>True if the polygon has no points, false otherwise.</returns>
        public bool IsEmptyPolygon()
        {
            return points.Count == 0;
        }
    }
}
