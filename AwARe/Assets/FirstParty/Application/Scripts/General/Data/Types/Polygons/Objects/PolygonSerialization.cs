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
        public float x, y, z;

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
        /// The points representing the polygon in the current session.
        /// </summary>
        [FormerlySerializedAs("listpoints")] public List<Vector3Serialization> sessionWorldPoints;

        /// <summary>
        /// The points saved relative to the first anchors. These are used to construct sessionWorldPoints for each session.
        /// </summary>
        [FormerlySerializedAs("polarpoints")] public List<Vector3Serialization> polarPoints;

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">points to use in the object.</param>
        public PolygonSerialization(Polygon polygon)
        {
            float height = polygon?.height ?? default;
            sessionWorldPoints = polygon?.points?.Select(v => new Vector3Serialization(v)).ToList() ?? new List<Vector3Serialization>();
        }

        // Add a parameterless constructor for deserialization
        [JsonConstructor]
        public PolygonSerialization()
        {
            sessionWorldPoints = new List<Vector3Serialization>();
        }
        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">points to use in the object.</param>
        public PolygonSerialization(List<Vector3Serialization> points)
        {
            this.sessionWorldPoints = points ?? new List<Vector3Serialization>();
        }

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of Vector3, converting them to serialized form.
        /// </summary>
        /// <param name="points">List of Vector3 representing polygon vertices.</param>
        public PolygonSerialization(List<Vector3> points)
        {
            this.sessionWorldPoints = points.Select(v => new Vector3Serialization(v)).ToList();
        }

        /// <summary>
        /// Converts the serialized polygon back to a Polygon object.
        /// </summary>
        /// <returns>The deserialized Polygon.</returns>
        public Polygon ToPolygon()
        {
            if (sessionWorldPoints == null)
            {
                Debug.LogError("points list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedlistpoints = sessionWorldPoints.Select(v => v.ToVector3()).ToList();
            return new Polygon(convertedlistpoints);
        }

        #region Point Conversion
        /*Process:
            1. When scanning the room you "hardcode" the initial world points based on that session.
            2. When saving the room you add two anchor points in that same session. 
                Then the world points get converted to polar points relative to the anchor points and save those polar points.
            3. Every time you load the room place the anchor points again in the new session.
            4. clear the last world points, recalculate them using the new anchor points of this session.
        */

        /// <summary>
        /// Calculate and set the polar points based on the initial world point values and the initial anchors.
        /// </summary>
        /// <param name="anchors">List of anchors for the current session.</param>
        public void GetPolarPoints(List<Vector3> anchors)
        {
            polarPoints = new List<Vector3Serialization>();
            foreach (Vector3Serialization worldPoint_ in sessionWorldPoints)
            {
                Vector3 worldPoint = worldPoint_.ToVector3();
                polarPoints.Add(WorldToPolar(worldPoint, anchors));
            }

        }

        /// <summary>
        /// Calculate and set the world points for a respective session.
        /// </summary>
        /// <param name="anchors">List of anchors for the current session.</param>
        public void GetSessionWorldPoints(List<Vector3> anchors)
        {
            sessionWorldPoints = new List<Vector3Serialization>();
            foreach (Vector3Serialization polarPoint in polarPoints)
            {
                sessionWorldPoints.Add(PolarToWorld(polarPoint, anchors));
            }
        }

        /// <summary>
        /// Calculate a polar point based on a world point.
        /// </summary>
        /// <param name="worldPoint">The worldpoint to be converted.</param>
        /// <param name="anchors">List of anchors for the current session.</param>
        /// <returns>Polar point as Vector3Serialization (r, alpha, _).</returns>
        private Vector3Serialization WorldToPolar(Vector3 worldPoint, List<Vector3> anchors)
        {
            float r = GetVector2Distance(anchors[1], worldPoint);
            float alpha = GetVector2Alpha(anchors[1], worldPoint);
            float anchorAlpha = GetVector2Alpha(anchors[0], anchors[1]);

            Vector3 polarPoint = new(r, alpha - anchorAlpha, 0);
            return new Vector3Serialization(polarPoint);
        }

        /// <summary>
        /// Calculate a world point based on a polar point.
        /// </summary>
        /// <param name="polarPoint">The polarpoint to be converted to a worldpoint.</param>
        /// <param name="anchors">List of anchors for the current session.</param>
        /// <returns>World point for the current session as a Vector3Serialization.</returns>
        private Vector3Serialization PolarToWorld(Vector3Serialization polarPoint, List<Vector3> anchors)
        {
            float r = polarPoint.x;
            float alpha = polarPoint.y;
            float anchorAlpha = GetVector2Alpha(anchors[0], anchors[1]);

            float wX = r * Mathf.Cos(alpha + anchorAlpha);
            float wZ = r * Mathf.Sin(alpha + anchorAlpha);

            Vector3 worldPoint = new(anchors[1].x + wX, anchors[1].y, anchors[1].z + wZ);

            return new Vector3Serialization(worldPoint);
        }

        /// <summary>
        /// Get the 2D distance between two Vector3 points. 2D on the floor, which translates to (x, z).
        /// </summary>
        /// <param name="a">The initial point.</param>
        /// <param name="b">The target point.</param>
        /// <returns>2D distance.</returns>
        private float GetVector2Distance(Vector3 a, Vector3 b)
        {
            float dX = b.x - a.x;
            float dZ = b.z - a.z;

            float r = Mathf.Sqrt(dX * dX + dZ * dZ);

            return r;
        }

        /// <summary>
        /// Get the 2D angle between two Vector3 points. 2D on the floor, which translates to (x, z).
        /// </summary>
        /// <param name="a">The initial point.</param>
        /// <param name="b">The target point.</param>
        /// <returns>2D alpha (angle).</returns>
        public float GetVector2Alpha(Vector3 a, Vector3 b)
        {
            float dX = b.x - a.x;
            float dZ = b.z - a.z;
            float alpha = Mathf.Atan(dZ / dX);

            if (dX < 0 && dZ >= 0)
                return alpha + Mathf.PI;
            if (dX < 0 && dZ < 0)
                return alpha - Mathf.PI;

            //if (dX >= 0 && dY >= 0)
            //|| (dX >= 0 && dY < 0)
            return alpha;
        }
        #endregion
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
        /// <param name="anchors">List of anchors for the current session.</param>
        /// <returns>The deserialized Room.</returns>
        public Room ToRoom(List<Vector3> anchors)
        {
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
