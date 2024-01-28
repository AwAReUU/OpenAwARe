// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// Class <c>PolygonSerialization</c> is responsible for serializing polygons represented by <see cref="Vector3"/>'s to a Room.
    /// </summary>
    [System.Serializable]
    public class PolygonSerialization
    {
        /// <summary>
        /// The height of the polygon in the current session.
        /// </summary>
        public float height;

        /// <summary>
        /// The points representing the polygon in the current session.
        /// </summary>
        [FormerlySerializedAs("listpoints")] public List<Vector3Serialization> sessionWorldPoints;

        /// <summary>
        /// The points saved relative to the first anchors. These are used to construct sessionWorldPoints for each session.
        /// </summary>
        [FormerlySerializedAs("polarpoints")] public List<Vector2Serialization> polarPoints;

        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        /// </summary>
        /// <param name="points">points to use in the object.</param>
        public PolygonSerialization(Logic.Polygon polygon)
        {
            height = polygon?.height ?? default;
            sessionWorldPoints = polygon?.points?.Select(v => new Vector3Serialization(v)).ToList() ?? new List<Vector3Serialization>();
        }

        // Add a parameterless constructor for deserialization
        [JsonConstructor]
        public PolygonSerialization()
        {
            height = default;
            sessionWorldPoints = new List<Vector3Serialization>();
        }

        /// <summary>
        /// Converts the serialized polygon back to a Polygon object.
        /// </summary>
        /// <returns>The deserialized Polygon.</returns>
        public Logic.Polygon ToPolygon()
        {
            if (sessionWorldPoints == null)
            {
                Debug.LogError("points list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedlistpoints = sessionWorldPoints.Select(v => v.ToVector3()).ToList();
            return new(convertedlistpoints, height);
        }

        #region Point Conversion
        /// <summary>
        /// Calculate and set the polar points based on the initial world point values and the initial anchors.
        /// </summary>
        /// <param name="anchors">List of anchors for the current session.</param>
        public void GetPolarPoints(List<Vector3> anchors)
        {
            polarPoints = new List<Vector2Serialization>();
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
            foreach (Vector2Serialization polarPoint in polarPoints)
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
        private Vector2Serialization WorldToPolar(Vector3 worldPoint, List<Vector3> anchors)
        {
            float r = GetVector2Distance(anchors[1], worldPoint);
            float alpha = GetVector2Alpha(anchors[1], worldPoint);
            float anchorAlpha = GetVector2Alpha(anchors[0], anchors[1]);

            Vector3 polarPoint = new(r, alpha - anchorAlpha, 0);
            
            return new Vector2Serialization(polarPoint);
        }

        /// <summary>
        /// Calculate a world point based on a polar point.
        /// </summary>
        /// <param name="polarPoint">The polarpoint to be converted to a worldpoint.</param>
        /// <param name="anchors">List of anchors for the current session.</param>
        /// <returns>World point for the current session as a Vector3Serialization.</returns>
        private Vector3Serialization PolarToWorld(Vector2Serialization polarPoint, List<Vector3> anchors)
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
}
