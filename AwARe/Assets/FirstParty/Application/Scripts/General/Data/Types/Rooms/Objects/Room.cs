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
    public class Room : MonoBehaviour, IDataHolder<Logic.Room>
    {
        /// <summary>
        /// Gets the data-type <see cref="Logic.Room"/> represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type <see cref="Logic.Room"/> represented.
        /// </value>
        public Logic.Room Data =>
            new(positivePolygon ? positivePolygon.Data : null, negativePolygons.Select(x => x.Data).ToList());
        
        /// <summary>
        /// The main polygon.
        /// </summary>
        public Polygon positivePolygon;

        /// <summary>
        /// The subtracted polygons.
        /// </summary>
        public List<Polygon> negativePolygons;

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="data">The data to store/represent.</param>
        /// <returns>The added component.</returns>
        public static Room AddComponentTo(GameObject gameObject, Logic.Room data)
        {
            Polygon positivePolygon = Polygon.AddComponentTo(new("Positive Polygon"), data.PositivePolygon);
            List<Polygon> negativePolygons = data.NegativePolygons.Select(polygon => Polygon.AddComponentTo(new("Negative Polygon"), polygon)).ToList();
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
            room.positivePolygon = positivePolygon;
            positivePolygon.transform.SetParent(room.transform, true);
            room.negativePolygons = negativePolygons;
            foreach (Polygon polygon in negativePolygons)
                polygon.transform.SetParent(room.transform, true);
            return room;
        }
    }
}