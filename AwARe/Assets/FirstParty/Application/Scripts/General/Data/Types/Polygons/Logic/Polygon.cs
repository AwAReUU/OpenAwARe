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
using UnityEngine.Serialization;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A Polygon representing (a part of) the floor.
    /// </summary>
    [Serializable]
    public class Polygon : IEquatable<Polygon>
    {
        /// <summary>
        /// Gets the area of the polygon.
        /// </summary>
        /// <value>The area of the polygon.</value>
        /// The points representing the polygon.
        /// </summary>
        public List<Vector3> points;

        /// <summary>
        /// Gets or sets the height of the Polygon cylinder.
        /// </summary>
        /// <value>
        /// The height of the Polygon cylinder in 3D space.
        /// </value>
        public float height;

        /// <summary>
        /// Calculates the area of the polygon.
        /// </summary>
        /// <returns>The area of the polygon.</returns>
        public float Area
        {
            get
            {
                float area = 0f;
                for (int i = 0, j = points.Count - 1; i < points.Count; i++)
                {
                    area += (points[j].x + points[i].x) * (points[j].z - points[i].z);
                    j = i;
                }
                
                return Mathf.Abs(area / 2);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">A list of pre-defined points of which the Polygon consists.</param>
        /// <param name="height">The height of the Polygon cylinder in 3D space.</param>
        public Polygon(List<Vector3> points = null, float height = 1f)
        {
            this.points = points ?? new();
            this.height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// Creates a deep copy of a Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to copy.</param>
        /// <returns>A Deep copy of <paramref name="polygon"/>.</returns>
        private Polygon(Polygon polygon)
        {
            points = new(polygon.points);
            height = polygon.height;
        }

        /// <inheritdoc/>
        public bool Equals(Polygon other) =>
            height.Equals(other.height) &&
            points.SequenceEqual(other.points);
        
        /// <summary>
        /// Creates a deep copy of this Polygon.
        /// </summary>
        /// <returns>A Deep copy of this Polygon.</returns>
        public Polygon Clone() => new(this);
    }
}
