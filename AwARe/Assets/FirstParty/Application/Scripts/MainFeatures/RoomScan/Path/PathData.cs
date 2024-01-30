// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// This class hold the data for a generated path.
    /// </summary>
    [Serializable]
    public class PathData
    {
        /// <summary>
        /// Each node on the path.
        /// </summary>
        public List<Vector3> points;

        /// <summary>
        /// Each segment/edge of the path.
        /// </summary>
        public List<(Vector3, Vector3)> edges;

        /// <summary>
        /// The radius around the skeleton of the path.
        /// </summary>
        public float radius = 0.2f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathData"/> class.
        /// </summary>
        public PathData()
        {
            points = new List<Vector3>();
            edges = new List<(Vector3, Vector3)>();
        }

        /// <summary>
        /// Checks whether the given point lies on the path.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Whether the given point lies on the path.</returns>
        public bool PointLiesOnPath(Vector3 point)
        {
            foreach (var edge in edges)
            {
                if (DistancePointToLineSegment(new Vector2(point.x, point.z), new Vector2(edge.Item1.x, edge.Item1.z), new Vector2(edge.Item2.x, edge.Item2.z)) <= radius)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the distance from a point to a line segment.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <param name="a">The start point of the line.</param>
        /// <param name="b">The end point of the line.</param>
        /// <returns>The distance from the point to the line segment.</returns>
        static double DistancePointToLineSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            var A = p.x - a.x;
            var B = p.y - a.y;
            var C = b.x - a.x;
            var D = b.y - a.y;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            var param = -1f;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = a.x;
                yy = a.y;
            }
            else if (param > 1)
            {
                xx = b.x;
                yy = b.y;
            }
            else
            {
                xx = a.x + param * C;
                yy = a.y + param * D;
            }

            var dx = p.x - xx;
            var dy = p.y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}