// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// A GameObject component containing data data.
    /// </summary>
    public class Room : MonoBehaviour
    {
        /// <summary>
        /// Gets the data-type <see cref="Logic.Room"/> represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type <see cref="Logic.Room"/> represented.
        /// </value>
        public Logic.Room Data =>
            new(PositivePolygon ? PositivePolygon.Data : null, NegativePolygons.Select(x => x.Data).ToList());
        
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

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="data">The data to store/represent.</param>
        /// <returns>The added component.</returns>
        public static Room AddComponentTo(GameObject gameObject, Logic.Room data)
        {
            var positivePolygon = new GameObject("Polygon").AddComponent<Polygon>();
            positivePolygon.Data = data.PositivePolygon;

            List<Polygon> negativePolygons = new();
            foreach (var polygon in data.NegativePolygons)
            {
                var negativePolygon = new GameObject("Polygon").AddComponent<Polygon>();
                positivePolygon.Data = polygon;
                negativePolygons.Add(negativePolygon);
            }

            return AddComponentTo(gameObject, positivePolygon, negativePolygons);
        }
        
        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="positivePolygon">The positive polygon of the data.</param>
        /// <param name="negativePolygons">The negative polygons of the data.</param>
        /// <returns>The added component.</returns>
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
    }
}