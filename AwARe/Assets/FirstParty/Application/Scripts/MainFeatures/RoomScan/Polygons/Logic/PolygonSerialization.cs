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
            public float x, y, z;

            public Vector3Serialization(UnityEngine.Vector3 vector)
            {
                x = vector.x;
                y = vector.y;
                z = vector.z;
            }

            public UnityEngine.Vector3 ToVector3()
            {
                return new UnityEngine.Vector3(x, y, z);
            }
        }

        public List<Vector3Serialization> Points;

        public PolygonSerialization(List<UnityEngine.Vector3> points)
        {
            Points = points.Select(v => new Vector3Serialization(v)).ToList();
        }

        public Polygon ToPolygon()
        {
            List<UnityEngine.Vector3> points = Points.Select(p => p.ToVector3()).ToList();
            return new Polygon(points);
        }
    }

}
