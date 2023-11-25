using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomScan
{
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
