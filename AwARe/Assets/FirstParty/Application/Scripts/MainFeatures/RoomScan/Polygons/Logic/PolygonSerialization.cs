using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.Data.Logic;
using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace AwARe
{

    [System.Serializable]
    public class Vector3Serialization
    {
        public float x;
        public float y;
        public float z;

        /// Constructor for Vector3Serialization, initializes the object with a Vector3.
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

    [System.Serializable]
    public class PolygonSerialization
    {
        public List<Vector3Serialization> PolarPoints;
        public List<Vector3Serialization> SessionWorldPoints;

        /// Constructor for PolygonSerialization, initializes the object with a list of serialized Vector3.
        public PolygonSerialization(List<Vector3Serialization> points)
        {
            SessionWorldPoints = points ?? new List<Vector3Serialization>();
        }
        /// <summary>
        /// Constructor for PolygonSerialization, initializes the object with a list of Vector3, converting them to serialized form.
        /// </summary>
        /// <param name="points">List of Vector3 representing polygon vertices.</param>
        public PolygonSerialization(List<Vector3> points)
        {
            SessionWorldPoints = points.Select(v => new Vector3Serialization(v)).ToList();
        }
        /// <summary>
        /// Converts the serialized polygon back to a Polygon object.
        /// </summary>
        /// <returns>The deserialized Polygon.</returns>
        public Polygon ToPolygon()
        {
            if (SessionWorldPoints == null)
            {
                Debug.LogError("Points list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedPoints = SessionWorldPoints.Select(v => v.ToVector3()).ToList();
            return new Polygon(convertedPoints);
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
        public void GetPolarPoints(Vector3Serialization anchor2)
        {
            PolarPoints.Clear();
            foreach (Vector3Serialization worldPoint in SessionWorldPoints)
            {
                PolarPoints.Add(WorldToPolar(worldPoint, anchor2));
            }
        }

        public void GetSessionWorldPoints(Vector3Serialization anchor1, Vector3Serialization anchor2)
        {
            SessionWorldPoints.Clear();
            foreach (Vector3Serialization polarPoint in PolarPoints)
            {
                SessionWorldPoints.Add(PolarToWorld(polarPoint, anchor1, anchor2));
            }
        }

        private Vector3Serialization WorldToPolar(Vector3Serialization worldPoint, Vector3Serialization anchor2)
        {
            float r = GetVector2Distance(anchor2, worldPoint);
            float alpha = GetVector2Alpha(anchor2, worldPoint);

            Vector3 polarPoint = new(r, worldPoint.y, alpha);
            return new Vector3Serialization(polarPoint);
        }

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
        
        private float GetVector2Distance(Vector3Serialization a, Vector3Serialization b)
        {
            float dX = b.x - a.x;
            float dY = b.y - a.y;
            float r = Mathf.Sqrt(dX * dX + dY * dY);

            return r;
        }

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
        public RoomSerialization(PolygonSerialization positivePolygon_, List<PolygonSerialization> negativePolygons_, List<Vector3Serialization> anchors_)
        {
            PositivePolygon = positivePolygon_;
            NegativePolygons = negativePolygons_;
            Anchors = anchors_;

            PositivePolygon.GetPolarPoints(Anchors[1]);
            foreach (PolygonSerialization poly in NegativePolygons)
            {
                poly.GetPolarPoints(Anchors[1]);
            }
        }

//TODO: Make it so that when loading we load using new anchors and GetWorldPoints() accordingly

        /// <summary>
        /// Converts the serialized room back to a Room object.
        /// </summary>
        /// <returns>The deserialized Room.</returns>
        // public Room ToRoom(List<Vector3> anchors_)
        // {
        //     PolygonSerialization positivePolygon = new PolygonSerialization();
        //     positivePolygon.GetSessionWorldPoints(new Vector3Serialization(anchors_[0]), new Vector3Serialization(anchors_[1]));
        //     positivePolygon = PositivePolygon.ToPolygon();
        //     List<Polygon> NegativePolygons = negativePolygons.Select(polygonSerialization => polygonSerialization.ToPolygon()).ToList();
        //     List<Vector3> anchors = Anchors.Select(a => a.ToVector3()).ToList();
        //     return new Room(positivePolygon, negativePolygons, anchors);
        // }

        /// <summary>
        /// Converts the serialized room back to a Room object.
        /// </summary>
        /// <returns>The deserialized Room.</returns>
        public Room ToRoom()
        {
            Polygon positivePolygon = PositivePolygon.ToPolygon();
            List<Polygon> negativePolygons = NegativePolygons.Select(polygonSerialization => polygonSerialization.ToPolygon()).ToList();
            List<Vector3> anchors = Anchors.Select(a => a.ToVector3()).ToList();
            return new Room(positivePolygon, negativePolygons, anchors);
        }
    }
}
