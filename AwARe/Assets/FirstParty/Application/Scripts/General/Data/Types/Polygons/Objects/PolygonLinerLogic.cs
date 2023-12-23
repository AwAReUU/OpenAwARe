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
    /// <summary>
    /// A line constructor class for the referred polygon.
    /// </summary>
    [Serializable]
    public class PolygonLinerLogic : MonoBehaviour, ILiner
    {
        /// <summary>
        /// Indicates if the last edge is rendered as well, closing the line.
        /// </summary>
        public bool closedLine;

        /// <summary>
        /// The polygon to represent.
        /// </summary>
        public Polygon polygon;
        private Logic.Polygon Data => polygon.Data;

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="polygon">A polygon.</param>
        /// <returns>The added component.</returns>
        public static PolygonLinerLogic AddComponentTo(GameObject gameObject, Polygon polygon)
        {
            var logic = gameObject.AddComponent<PolygonLinerLogic>();
            logic.polygon = polygon;
            return logic;
        }

        /// <inheritdoc/>
        public Vector3[] Line =>
            ConstructLine(Data);

        /// <summary>
        /// Constructs a line representing the given Polygon. <br/>
        /// If the Polygon is set to null, then return an empty line.
        /// </summary>
        /// <param name="polygon">The Polygon to construct a line from.</param>
        /// <returns>A line representing the given Polygon.</returns>
        private Vector3[] ConstructLine(Logic.Polygon polygon = null) =>
            // Handle null input.
            polygon == null ? Array.Empty<Vector3>() : ConstructLine_Body(polygon);

        /// <summary>
        /// Constructs a line representing the given Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to construct a line from.</param>
        /// <returns>A line representing the given Polygon.</returns>
        private Vector3[] ConstructLine_Body(Logic.Polygon polygon)
        {
            List<Vector3> points = new(polygon.Points);
            if(closedLine && points.Count > 2) { points.Add(points[0]); Debug.Log("ClosedPointAdded");}
            // Get the data.
            return points.ToArray();
        }
    }
}