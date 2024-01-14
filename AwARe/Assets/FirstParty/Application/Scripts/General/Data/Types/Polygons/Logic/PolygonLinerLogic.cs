// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AYellowpaper;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A line constructor class for the referred polygon.
    /// </summary>
    [Serializable]
    public class PolygonLinerLogic : MonoBehaviour, ILinerLogic
    {
        /// <summary>
        /// Indicates if the last edge is rendered as well, closing the line.
        /// </summary>
        public bool closedLine;
        
        /// <summary>
        /// The polygon to represent.
        /// </summary>
        public InterfaceReference<IDataHolder<Polygon>> polygon;

        /// <summary>
        /// Gets the data of the polygon.
        /// </summary>
        /// <value>The data of the polygon.</value>
        private Polygon Data
        {
            get
            {
                var value = polygon.Value;
                return value.Data;
            }
        }

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="polygon">A polygon.</param>
        /// <param name="closedLine">False if the last (closing) line should be omitted.</param>
        /// <returns>The added component.</returns>
        public static PolygonLinerLogic AddComponentTo(GameObject gameObject, InterfaceReference<IDataHolder<Polygon>> polygon, bool closedLine = true)
        {
            var logic = gameObject.AddComponent<PolygonLinerLogic>();
            logic.polygon = polygon;
            logic.closedLine = closedLine;
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
        private Vector3[] ConstructLine(Polygon polygon = null) =>
            // Handle null input.
            polygon == null ? Array.Empty<Vector3>() : ConstructLine_Body(polygon);

        /// <summary>
        /// Constructs a line representing the given Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to construct a line from.</param>
        /// <returns>A line representing the given Polygon.</returns>
        private Vector3[] ConstructLine_Body(Polygon polygon)
        {
            List<Vector3> points = new(polygon.points);
            if(closedLine && points.Count > 2) { points.Add(points[0]); Debug.Log("ClosedPointAdded");}
            // Get the data.
            return points.ToArray();
        }
    }
}