using System.Collections;
using System.Collections.Generic;
using System.Linq;

using AwARe.Data.Logic;

using UnityEngine;

namespace AwARe
{

    [System.Serializable]
    public class PolygonSerialization
    {
        [System.Serializable]
        public class Vector3Serialization
        {
            public float x;
            public float y;
            public float z;

            public Vector3Serialization(Vector3 vector)
            {
                x = vector.x;
                y = vector.y;
                z = vector.z;
            }

            public Vector3 ToVector3()
            {
                return new Vector3(x, y, z);
            }
        }

        public List<Vector3Serialization> Points;

        public PolygonSerialization(List<Vector3Serialization> points)
        {
            Points = points ?? new List<Vector3Serialization>();
        }

        public PolygonSerialization(List<Vector3> points)
        {
            Points = points.Select(v => new Vector3Serialization(v)).ToList();
        }

        public Polygon ToPolygon()
        {
            if (Points == null)
            {
                Debug.LogError("Points list is null when converting to Polygon.");
                return null;
            }

            List<Vector3> convertedPoints = Points.Select(v => v.ToVector3()).ToList();
            return new Polygon(convertedPoints);
        }
    }
}
