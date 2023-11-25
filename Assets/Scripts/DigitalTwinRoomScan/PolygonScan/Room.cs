// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

namespace RoomScan
{
    /// <summary>
    /// A scanned room consisting of polygons.
    /// </summary>
    public class Room
    {
        /// <summary>
        /// Gets or sets the polygon covering the entire room.
        /// </summary>
        public Polygon PositivePolygon { get; set; }

        /// <summary>
        /// Gets or sets the list of polygons that have to be 'cut out' of the positive polygon (e.g. tables).
        /// </summary>
        public List<Polygon> NegativePolygons { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Room"/> class.
        /// Either fills it with the given polygons or creates new ones.
        /// </summary>
        /// <param name="posPolygon">The positive polygon of the room.</param>
        /// <param name="negPolygons">The negative polygon of the room.</param>
        public Room(Polygon posPolygon = null, List<Polygon> negPolygons = null)
        { 
            if(posPolygon != null)
                PositivePolygon = posPolygon;
            else
                PositivePolygon = new Polygon();

            if(negPolygons != null)
                NegativePolygons = negPolygons;
            else
                NegativePolygons = new List<Polygon>();
        }

        /// <summary>
        /// Add a polygon to the room; if it is the forst polygon it is the positive polygon, otherwise it is a negative polygon.
        /// </summary>
        /// <param name="polygon">The polygon to add to the room.</param>
        public void AddPolygon(Polygon polygon)
        {
            if (PositivePolygon == null)
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
