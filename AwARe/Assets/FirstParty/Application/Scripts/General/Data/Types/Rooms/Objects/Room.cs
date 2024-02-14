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

        public string roomName;
        /// <summary>
        /// The main polygon template.
        /// </summary>
        public Polygon positivePolygonBase;

        /// <summary>
        /// The subtracted polygon template.
        /// </summary>
        public Polygon negativePolygonBase;

        public float height;

        /// <summary>
        /// Gets the data-type <see cref="Logic.Room"/> represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type <see cref="Logic.Room"/> represented.
        /// </value>
        public Logic.Room Data
        {
            get => new Logic.Room(
                positivePolygon ? positivePolygon.Data : null,
                negativePolygons.Select(x => x.Data).ToList()
            )
            {
                RoomName = roomName, // Include roomName
            };
            set => SetComponent(value);
        }

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
            room.SetComponent(positivePolygon, negativePolygons);
            return room;
        }

        public void SetComponent(Polygon positivePolygonBase, Polygon negativePolygonBase, Logic.Room data)
        {
            this.positivePolygonBase = positivePolygonBase;
            this.negativePolygonBase = negativePolygonBase;
            SetComponent(data);
        }

        public void SetComponent(Logic.Room data)
        {
            if (data.PositivePolygon == null) return;

            Polygon SpawnPolygon(Polygon polygonBase, Logic.Polygon polygonData)
            {
                if (polygonData == null)
                    return null;

                var polygon = Instantiate(polygonBase.gameObject).GetComponent<Polygon>();
                polygon.SetComponent(polygonData);
                return polygon;
            }

            var positivePolygon = SpawnPolygon(this.positivePolygonBase, data.PositivePolygon);
            var negativePolygons = data.NegativePolygons.Select(polygon => SpawnPolygon(this.negativePolygonBase, polygon)).ToList();
            SetComponent(positivePolygon, negativePolygons);
            this.roomName = data.RoomName;
            Debug.Log($"Updated RoomName: {this.roomName}");
        }

        public void SetComponent(Polygon positive, List<Polygon> negatives)
        {
            this.positivePolygon = positive;
            positivePolygon.transform.SetParent(this.transform, true);
            this.negativePolygons = negatives;
            foreach (var polygon in negativePolygons)
                polygon.transform.SetParent(this.transform, true);
        }
    }
}