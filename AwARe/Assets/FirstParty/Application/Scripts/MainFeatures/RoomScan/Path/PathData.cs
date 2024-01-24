// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AwARe.RoomScan.Path
{
    /// <summary>
    /// This class hold the data for a generated path.
    /// </summary>
    [Serializable]
    public class PathData
    {
        /// <summary>
        /// Each node on the path.
        /// </summary>
        public List<Vector3> points;

        /// <summary>
        /// Each segment/edge of the path.
        /// </summary>
        public List<(Vector3, Vector3)> edges;

        /// <summary>
        /// The radius around the skeleton of the path.
        /// </summary>
        public float radius = 0.2f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PathData"/> class.
        /// </summary>
        public PathData()
        {
            points = new List<Vector3>();
            edges = new List<(Vector3, Vector3)>();
        }
    }
}