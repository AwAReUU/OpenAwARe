using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomScan
{
    public class Polygon
    {
        /// <summary>
        /// The points that form the 'corners' of the polygon.
        /// </summary>
        protected List<Vector3> points;

        /// <summary>
        /// Instantiates a polygon representing (a part of) the floor.
        /// </summary>
        public Polygon(List<Vector3> points = null)
        {
            if (points == null)
                this.points = new List<Vector3>();
            else
                this.points = points;
        }

        /// <summary>
        /// Adds the given point to the list of points
        /// </summary>
        /// <param name="point">The point to add to the list</param>
        public void AddPoint(Vector3 point)
        {
            points.Add(point);
        }

        /// <summary>
        /// Removes the last added point from the list of points
        /// </summary>
        public void RemoveLastPoint()
        {
            if (points.Count > 0)
            {
                this.points.RemoveAt(this.points.Count - 1);
            }
        }

        /// <summary>
        /// Gets the points list in array form
        /// </summary>
        /// <returns>An array containing the points of the point lsit</returns>
        public Vector3[] GetPoints()
        {
            return this.points.ToArray();
        }

        /// <summary>
        /// Gets the points list
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetPointsList()
        {
            return this.points;
        }

        /// <summary>
        /// Returns the amount of points
        /// </summary>
        /// <returns></returns>
        public int AmountOfPoints()
        {
            return points.Count;
        }

        /// <summary>
        /// Gets the first point from the points list
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFirstPoint()
        {
            return points[0];
        }
    }
}
