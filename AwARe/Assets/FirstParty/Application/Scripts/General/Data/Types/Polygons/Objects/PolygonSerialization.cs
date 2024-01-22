// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \* 

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
    /// Class <c>Vector3Serialization</c> is responsible for Serializing <see cref="Vector3"/> to <see cref="Vector3Serialization"/>.
    /// </summary>
    [System.Serializable]
    public class Vector3Serialization
    {
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// Constructor for Vector3Serialization, initializes the object with a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">Vector3 to use for initialization.</param>
        public Vector3Serialization(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        /// <summary>
        /// Converts the serialized Vector3 back to a Vector3 object.
        /// </summary>
        /// <returns>The deserialized Vector3.</returns>
        public Vector3 ToVector3() =>
            new(x, y, z);
    }

    /// <summary>
    /// Class <c>PolygonSerialization</c> is responsible for serializing polygons represented by <see cref="Vector3"/>'s to a Room.
    /// </summary>
    [System.Serializable]
    public class PolygonSerialization
    {
        /// <summary>
        /// The points representing the polygon.
        /// </summary>
        [FormerlySerializedAs("listpoints")] public List<Vector3Serialization> points;

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">points to use in the object.</param>
        public PolygonSerialization(Polygon polygon)
        {
            float height = polygon?.height ?? default;
            points = polygon?.points?.Select(v => new Vector3Serialization(v)).ToList() ?? new List<Vector3Serialization>();
        }

        // Add a parameterless constructor for deserialization
        [JsonConstructor]
        public PolygonSerialization()
        {
            points = new List<Vector3Serialization>();
        }
        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">points to use in the object.</param>
        public PolygonSerialization(List<Vector3Serialization> points)
        {
            this.points = points ?? new List<Vector3Serialization>();
        }

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of Vector3, converting them to serialized form.
        /// </summary>
        /// <param name="points">List of Vector3 representing polygon vertices.</param>
        public PolygonSerialization(List<Vector3> points)
        {
            this.points = points.Select(v => new Vector3Serialization(v)).ToList();
        }

        /// <summary>
        /// Converts the serialized polygon back to a Polygon object.
        /// </summary>
        /// <returns>The deserialized Polygon.</returns>
        public Polygon ToPolygon()
        {
            if (points == null)
            {
                Debug.LogError("points list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedlistpoints = points.Select(v => v.ToVector3()).ToList();
            return new Polygon(convertedlistpoints);
        }
    }

    /// <summary>
    /// Class <c>RoomSerialization</c> is responsible for Serializing <see cref="Polygon"/>s to a <see cref="Room"/>.
    /// </summary>
    [System.Serializable]
    public class RoomSerialization
    {
        public PolygonSerialization PositivePolygon;
        public List<PolygonSerialization> NegativePolygons;
        public string RoomName;
        public float RoomHeight;



        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        public RoomSerialization(Room room)
        {
            RoomName="hello";
            RoomHeight = room.PositivePolygon?.height ?? default;
            PositivePolygon = new(room.PositivePolygon);
            NegativePolygons = room.NegativePolygons.Select(polygon => new PolygonSerialization(polygon)).ToList();
        }

        // Add a parameterless constructor for deserialization
        [JsonConstructor]
        public RoomSerialization()
        {
            PositivePolygon = new PolygonSerialization();
            NegativePolygons = new List<PolygonSerialization>();
            RoomName = "";
            RoomHeight = 0;
        }

        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        public RoomSerialization(PolygonSerialization positivePolygon, List<PolygonSerialization> negativePolygons, string roomName, int roomHeight)
        {
            RoomName = roomName;
            RoomHeight = roomHeight;
            PositivePolygon = positivePolygon;
            NegativePolygons = negativePolygons;
        }

        /// <summary>
        /// Converts the serialized room back to a Room object.
        /// </summary>
        /// <returns>The deserialized Room.</returns>
        public Room ToRoom()
        {
            Polygon positivePolygon = PositivePolygon.ToPolygon();
            List<Polygon> negativePolygons = NegativePolygons.Select(polygonSerialization => polygonSerialization.ToPolygon()).ToList();
            return new Room(positivePolygon, negativePolygons, RoomName, RoomHeight);
        }
    }

    [System.Serializable]
    public class RoomListSerialization
    {
        public List<RoomSerialization> Rooms;

        public RoomListSerialization(List<RoomSerialization> rooms)
        {
            Rooms = rooms ?? new List<RoomSerialization>();
        }

        public RoomListSerialization()
        {
            Rooms = new List<RoomSerialization>();
        }

        // Add a constructor with a default value or empty parameters for deserialization
        [JsonConstructor]
        public RoomListSerialization(int dummy)
        {
            Rooms = new List<RoomSerialization>();
        }
    }

}
