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
        [FormerlySerializedAs("listpoints")] public List<Vector3Serialization> sessionWorldPoints;
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
                (TODO: bound the anchors to be the same distance every time or some form of enforcing the same location or ...(Seanas screenshot thing?))
            4. clear the last world points, recalculate them using the new anchor points of this session. == GetWorldPoints() and load those world points.
        */

        /// <summary>
        /// Calculate and set the polar points based on the initial world point values and the initial anchors.
        /// </summary>
        /// <param name="anchor2">The second anchor placed by the user.</param>
        public void GetPolarPoints(Vector3Serialization anchor2)
        {
            polarPoints.Clear();
            foreach (Vector3Serialization worldPoint in sessionWorldPoints)
            {
                polarPoints.Add(WorldToPolar(worldPoint, anchor2));
            }
        }

        /// <summary>
        /// Calculate and set the world points for a respective session.
        /// </summary>
        /// <param name="anchor1">The first anchor of the session, indicated by the user.</param>
        /// <param name="anchor2">The second anchor of the session, indicated by the user.</param>
        public void GetSessionWorldPoints(Vector3Serialization anchor1, Vector3Serialization anchor2)
        {
            sessionWorldPoints.Clear();
            foreach (Vector3Serialization polarPoint in polarPoints)
            {
                sessionWorldPoints.Add(PolarToWorld(polarPoint, anchor1, anchor2));
            }
        }

        /// <summary>
        /// Calculate a polar point based on a world point.
        /// </summary>
        /// <param name="worldPoint">The worldpoint to be converted.</param>
        /// <param name="anchor2">The second anchor of the session, indicated by the user.</param>
        /// <returns></returns>
        private Vector3Serialization WorldToPolar(Vector3Serialization worldPoint, Vector3Serialization anchor2)
        {
            float r = GetVector2Distance(anchor2, worldPoint);
            float alpha = GetVector2Alpha(anchor2, worldPoint);

            Vector3 polarPoint = new(r, worldPoint.y, alpha);
            return new Vector3Serialization(polarPoint);
        }

        /// <summary>
        /// Calculate a world point based on a polar point.
        /// </summary>
        /// <param name="polarPoint">The polarpoint to be converted to a worldpoint.</param>
        /// <param name="anchor1">The first anchor of the session, indicated by the user.</param>
        /// <param name="anchor2">The second anchor of the session, indicated by the user.</param>
        /// <returns></returns>
        private Vector3Serialization PolarToWorld(Vector3Serialization polarPoint, Vector3Serialization anchor1, Vector3Serialization anchor2)
        {
            float r = polarPoint.x;
            float alpha = polarPoint.y;
            float anchorAlpha = GetVector2Alpha(anchor1, anchor2);

            float wx = r * Mathf.Cos(alpha + anchorAlpha);
            float wz = r * Mathf.Sin(alpha + anchorAlpha);

            Vector3 worldPoint = anchor2.ToVector3() + new Vector3(wx, polarPoint.y, wz);

            return new Vector3Serialization(worldPoint);
        }

        /// <summary>
        /// Get the 2D distance between two points.
        /// </summary>
        /// <param name="a">The initial point.</param>
        /// <param name="b">The target point.</param>
        /// <returns>2D distance.</returns>
        private float GetVector2Distance(Vector3Serialization a, Vector3Serialization b)
        {
            float dX = b.x - a.x;
            float dY = b.y - a.y;
            float r = Mathf.Sqrt(dX * dX + dY * dY);

            return r;
        }

        /// <summary>
        /// Get the 2D angle between two points.
        /// </summary>
        /// <param name="a">The initial point.</param>
        /// <param name="b">The target point.</param>
        /// <returns>2D alpha (angle).</returns>
        private float GetVector2Alpha(Vector3Serialization a, Vector3Serialization b)
        {
            float dX = b.x - a.x;
            float dY = b.y - a.y;
            float alpha = Mathf.Atan(dY / dX);

            if (dX < 0 && dY >= 0)
                return alpha + Mathf.PI;
            if (dX < 0 && dY < 0)
                return alpha - Mathf.PI;

            //if (x >= 0 && y >= 0)
            //|| (x >= 0 && y < 0)
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
        public List<Vector3Serialization> Anchors;

        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        public RoomSerialization(Room room, List<Vector3> anchors_)
        {
            PositivePolygon = new(room.PositivePolygon);
            NegativePolygons = room.NegativePolygons.Select(polygon => new PolygonSerialization(polygon)).ToList();
            Anchors = anchors_.Select(a => new Vector3Serialization(a)).ToList();
            
            PositivePolygon.GetPolarPoints(Anchors[1]);
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetPolarPoints(Anchors[1]);
            }
        }

        /// <summary>
        /// Constructor for RoomSerialization, initializes the object with serialized positive and negative polygons.
        /// </summary>
        /// <param name="positivePolygon">Serialized positive polygon.</param>
        /// <param name="negativePolygons">List of serialized negative polygons.</param>
        /// <param name="anchors_">List of anchors for the current session.</param>
        public RoomSerialization(PolygonSerialization positivePolygon,
            List<PolygonSerialization> negativePolygons, List<Vector3Serialization> anchors_)
        {
            PositivePolygon = positivePolygon;
            NegativePolygons = negativePolygons;
            Anchors = anchors_;

            PositivePolygon.GetPolarPoints(Anchors[1]);
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetPolarPoints(Anchors[1]);
            }
        }

        /// <summary>
        /// Converts the serialized room back to a Room object.
        /// </summary>
        /// <returns>The deserialized Room.</returns>
        public Room ToRoom(List<Vector3> sessionAnchors)
        {
            Anchors = sessionAnchors.Select(a => new Vector3Serialization(a)).ToList();
            PositivePolygon.GetSessionWorldPoints(Anchors[0], Anchors[1]);
            Polygon positivePolygon = PositivePolygon.ToPolygon();
            
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetSessionWorldPoints(Anchors[0], Anchors[1]);
            }
            List<Polygon> negativePolygons = NegativePolygons.Select(polygonSerialization => polygonSerialization.ToPolygon()).ToList();
            
            return new Room(positivePolygon, negativePolygons);
        }
    }
}
