// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// A GameObject component containing room data.
    /// </summary>
    public class Room : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets and sets the data-type <see cref="Logic.Room"/> represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type <see cref="Logic.Room"/> represented.
        /// </value>
        public Logic.Room Data =>
            new(PositivePolygon.Data, NegativePolygons.Select(x => x.Data).ToList());

        public static Room AddComponentTo(GameObject gameObject, Logic.Room room)
        {
            Polygon positivePolygon = new GameObject("Polygon").AddComponent<Polygon>();
            positivePolygon.Data = room.PositivePolygon;

            List<Polygon> negativePolygons = new();
            foreach (var polygon in room.NegativePolygons)
            {
                Polygon negativePolygon = new GameObject("Polygon").AddComponent<Polygon>();
                positivePolygon.Data = polygon;
                negativePolygons.Add(negativePolygon);
            }

            return AddComponentTo(gameObject, positivePolygon, negativePolygons);
        }

        public static Room AddComponentTo(GameObject gameObject, Polygon positivePolygon, List<Polygon> negativePolygons)
        {
            var room = gameObject.AddComponent<Room>();
            room.PositivePolygon = positivePolygon;
            positivePolygon.transform.SetParent(room.transform, true);
            room.NegativePolygons = negativePolygons;
            foreach (Polygon polygon in negativePolygons)
                polygon.transform.SetParent(room.transform, true);

            return room;
        }
        
        /// <summary>
        /// Gets or sets the main polygon.
        /// </summary>
        /// <value>
        /// The main polygon.
        /// </value>
        public Polygon PositivePolygon { get; set; }

        /// <summary>
        /// Gets or sets the subtracted polygons.
        /// </summary>
        /// <value>
        /// The subtracted polygons.
        /// </value>
        public List<Polygon> NegativePolygons { get; set; } = new();
    }
}