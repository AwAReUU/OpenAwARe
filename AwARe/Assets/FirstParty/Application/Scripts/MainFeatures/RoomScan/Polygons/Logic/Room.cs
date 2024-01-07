// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;
using AwARe.Data.Logic;

namespace AwARe.RoomScan.Polygons.Logic
{
    /// <summary>
    /// A scanned room consisting of polygons.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets the polygon covering the entire room.
        /// </summary>
        /// <value>The polygon covering the entire room.</value>
        public Polygon PositivePolygon { get; set; }

        /// <summary>
        /// Gets or sets the list of polygons that have to be 'cut out' of the positive polygon (e.g. tables).
        /// </summary>
        /// /// <value>The polygons that have to be 'cut out' of the positive polygon.</value>
        public List<Polygon> NegativePolygons { get; set; }


        /// <summary>
        /// List containing the anchor points of the room.
        /// </summary>
        /// <value>The points used to load the polygon from a save.</value>
        public List<Vector3> Anchors = new();

        /// <summary>
        /// Adds an anchorpoint to the room. Anchors are used to load the room in the correct world location.
        /// </summary>
        /// <param name="anchor"></param>
        public void AddAnchor(Vector3 anchor) => Anchors.Add(anchor);

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// Either fills it with the given polygons or creates new ones.
        /// </summary>
        /// <param name="posPolygon">The positive polygon of the room.</param>
        /// <param name="negPolygons">The negative polygon of the room.</param>
        /// <param name="anchors">The anchor points of the room.</param>
        public Room(Polygon posPolygon = null, List<Polygon> negPolygons = null, List<Vector3> anchors = null)
        {
            if (posPolygon != null)
                PositivePolygon = posPolygon;
            else
                PositivePolygon = new Polygon();

            if (negPolygons != null)
                NegativePolygons = negPolygons;
            else
                NegativePolygons = new List<Polygon>();
            
            if (anchors != null)
                Anchors = anchors;
        }

        /// <summary>
        /// Add a polygon to the room; if it is the first polygon it is the positive polygon,
        /// otherwise it is a negative polygon.
        /// </summary>
        /// <param name="polygon">The polygon to add to the room.</param>
        public void AddPolygon(Polygon polygon)
        {
            if (PositivePolygon.IsEmptyPolygon())
            {
                PositivePolygon = polygon;
            }
            else
            {
                NegativePolygons.Add(polygon);
            }
        }
    }
}
