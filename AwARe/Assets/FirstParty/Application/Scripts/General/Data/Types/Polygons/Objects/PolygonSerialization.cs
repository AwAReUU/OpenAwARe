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

using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// Class <c>PolygonSerialization</c> is responsible for serializing polygons represented by <see cref="Vector3"/>'s to a Room.
    /// </summary>
    [System.Serializable]
    public class PolygonSerialization
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
            public Vector3 ToVector3()
            {
                return new Vector3(x, y, z);
            }
        }
        
        /// <summary>
        /// The points representing the polygon.
        /// </summary>
        public List<Vector3Serialization> listpoints;

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">Points to use in the object.</param>
        public PolygonSerialization(List<Vector3Serialization> points)
        {
            listpoints = points ?? new List<Vector3Serialization>();
        }

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of Vector3, converting them to serialized form.
        /// </summary>
        /// <param name="points">List of Vector3 representing polygon vertices.</param>
        public PolygonSerialization(List<Vector3> points)
        {
            listpoints = points.Select(v => new Vector3Serialization(v)).ToList();
        }

        /// <summary>
        /// Converts the serialized polygon back to a Polygon object.
        /// </summary>
        /// <returns>The deserialized Polygon.</returns>
        public Polygon ToPolygon()
        {
            if (listpoints == null)
            {
                Debug.LogError("listpoints list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedlistpoints = listpoints.Select(v => v.ToVector3()).ToList();
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

        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        public RoomSerialization(PolygonSerialization positivePolygon, List<PolygonSerialization> negativePolygons)
        {
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
            return new Room(positivePolygon, negativePolygons);
        }
    }
}
