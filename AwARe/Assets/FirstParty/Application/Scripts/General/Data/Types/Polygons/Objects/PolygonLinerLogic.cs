// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.Data.Objects
{
    [Serializable]
    public class PolygonLinerLogic : MonoBehaviour, ILiner
    {
        public bool closedLine;
        public Polygon polygon;

        private Logic.Polygon Data => polygon.Data;

        public PolygonLinerLogic(Polygon polygon, bool closedLine = true)
        {
            this.polygon = polygon;
            this.closedLine = closedLine;
        }

        /// <inheritdoc/>
        public Vector3[] Line =>
            ComputeLine(Data);

        /// <summary>
        /// Compute a line representing the given Polygon. <br/>
        /// If the Polygon is set to null, then return an empty line.
        /// </summary>
        /// <param name="polygon">The Polygon to compute a line from.</param>
        /// <returns>A line representing the given Polygon.</returns>
        private Vector3[] ComputeLine(Logic.Polygon polygon = null) =>
            // Handle null input.
            polygon == null ? Array.Empty<Vector3>() : ComputeLine_Body(polygon);

        /// <summary>
        /// Compute a line representing the given Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to compute a line from.</param>
        /// <returns>A line representing the given Polygon.</returns>
        private Vector3[] ComputeLine_Body(Logic.Polygon polygon)
        {
            List<Vector3> points = new(polygon.Points);
            if(closedLine && points.Count > 2) { points.Add(points[0]); Debug.Log("ClosedPointAdded");}
            // Get the data.
            return points.ToArray();
        }
    }
}