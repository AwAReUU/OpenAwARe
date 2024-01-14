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

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A Polygon representing (a part of) the floor.
    /// </summary>
    [Serializable]
    public class Polygon : IEquatable<Polygon>
    {
        /// <summary>
        /// Gets or sets the points of the Polygon.
        /// </summary>
        /// <value>
        /// The points that form the 'corners' of the Polygon.
        /// </value>
        public List<Vector3> Points { get => listpoints; set => listpoints = value; }
        
        /// <summary>
        /// Gets or sets the height of the Polygon cylinder.
        /// </summary>
        /// <value>
        /// The height of the Polygon cylinder in 3D space.
        /// </value>
        public float Height { get; set; }

        /// <summary>
        /// Gets the area of the polygon.
        /// </summary>
        /// <value>The area of the polygon.</value>
        /// The points representing the polygon.
        /// </summary>
        public List<Vector3> listpoints;

        /// <summary>
        /// Gets the surface area of the polygon.
        /// </summary>
        public float Area
        {
            get
            {
                float area = 0f;
                int j = listpoints.Count - 1;

                for (int i = 0; i < listpoints.Count; i++)
                {
                    area += (Points[j].x + Points[i].x) * (Points[j].z - Points[i].z);
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
            Points = points ?? new();
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// Creates a deep copy of a Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to copy.</param>
        /// <returns>A Deep copy of <paramref name="polygon"/>.</returns>
        private Polygon(Polygon polygon)
        {
            Points = new(polygon.Points);
            Height = polygon.Height;
        }

        /// <inheritdoc/>
        public bool Equals(Polygon other) =>
            Height.Equals(other.Height) &&
            Points.SequenceEqual(other.Points);
        
        /// <summary>
        /// Creates a deep copy of this Polygon.
        /// </summary>
        /// <returns>A Deep copy of this Polygon.</returns>
        public Polygon Clone() => new(this);
    }
}
