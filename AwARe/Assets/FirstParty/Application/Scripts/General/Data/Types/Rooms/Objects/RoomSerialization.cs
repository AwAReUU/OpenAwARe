using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe
{
    /// <summary>
    /// Class <c>RoomSerialization</c> is responsible for Serializing <see cref="Polygon"/>s to a <see cref="Room"/>.
    /// </summary>
    [System.Serializable]
    public class RoomSerialization
    {
        /// <summary>
        /// The main polygon of the room.
        /// </summary>
        public PolygonSerialization PositivePolygon;

        /// <summary>
        /// The negative polygons (if any) to disable spawns within the main polygon.
        /// </summary>
        public List<PolygonSerialization> NegativePolygons;

        /// <summary>
        /// The name of the room.
        /// </summary>
        public string RoomName;

        /// <summary>
        /// The height of the room.
        /// </summary>
        public float RoomHeight;

        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="room">The room object for the current session.</param>
        /// <param name="anchors">List of anchors for the current session.</param>
        public RoomSerialization(Room room, List<Vector3> anchors)
        {
            RoomName = room.RoomName;
            RoomHeight = room.PositivePolygon?.height ?? default;
            PositivePolygon = new(room.PositivePolygon);
            NegativePolygons = room.NegativePolygons.Select(polygon => new PolygonSerialization(polygon)).ToList();

            PositivePolygon.GetPolarPoints(anchors);
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetPolarPoints(anchors);
            }
        }

        /// <summary>
        /// Add a parameterless constructor for deserialization.
        /// </summary>
        [JsonConstructor]
        public RoomSerialization()
        {
            PositivePolygon = new PolygonSerialization();
            NegativePolygons = new List<PolygonSerialization>();
            RoomName = "";
            RoomHeight = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomSerialization"/> class,
        /// with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        /// <param name="anchors">List of anchors for the current session.</param>
        /// <param name="roomName">Name of the room.</param>
        /// <param name="roomHeight">Height of the room.</param>
        /// </summary>
        public RoomSerialization(PolygonSerialization positivePolygon, List<PolygonSerialization> negativePolygons,
            List<Vector3> anchors, string roomName, int roomHeight)
        {
            RoomName = roomName;
            RoomHeight = roomHeight;
            PositivePolygon = positivePolygon;
            NegativePolygons = negativePolygons;

            PositivePolygon.GetPolarPoints(anchors);
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetPolarPoints(anchors);
            }
        }

        /// <summary>
        /// Converts the serialized room back to a Room object.
        /// </summary>
        /// <param name="anchors_">List of anchors for the current session.</param>
        /// <returns>The deserialized Room.</returns>
        public Room ToRoom(List<Vector3> anchors_)
        {
            List<Vector3> anchors = new List<Vector3> { Vector3.zero, Vector3.zero };

            if (anchors_.Count < 2)
                Debug.LogError("Not enough anchors");
            else
                anchors = anchors_;

            PositivePolygon.GetSessionWorldPoints(anchors);

            foreach (PolygonSerialization p in NegativePolygons)
            {
                p.GetSessionWorldPoints(anchors);
            }

            Polygon positivePolygon = PositivePolygon.ToPolygon();
            List<Polygon> negativePolygons = NegativePolygons.Select(polygonSerialization => polygonSerialization.ToPolygon()).ToList();
            return new Room(positivePolygon, negativePolygons, RoomName, RoomHeight);
        }
    }

    [System.Serializable]
    public class RoomListSerialization
    {
        /// <summary>
        /// The serialized rooms.
        /// </summary>
        public List<RoomSerialization> Rooms;

        /// <summary>
        /// Constructor for a list of serialized rooms.
        /// </summary>
        /// <param name="rooms">Serialized rooms to be added to the list.</param>
        public RoomListSerialization(List<RoomSerialization> rooms = null)
        {
            Rooms = rooms ?? new List<RoomSerialization>();
        }
    }
}
