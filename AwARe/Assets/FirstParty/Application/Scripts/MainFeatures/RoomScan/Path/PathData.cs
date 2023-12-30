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

        /// <summary>
        /// Create a Mesh of the path including the surrounding radius.
        /// </summary>
        /// <param name="numSegments">The number of segments in the mesh.</param>
        /// <returns>The created Mesh.</returns>
        public Mesh CreateMesh(int numSegments)
        {

            Mesh mesh = new();

            for (int i = 0; i < points.Count; i++)
            {
                Mesh circle = CircleMesh(points[i], this.radius, numSegments);
                mesh = CombineMeshes(mesh, circle);
            }

            for (int i = 0; i < edges.Count; i++)
            {
                Mesh segment = this.SegmentMesh(edges[i].Item1, edges[i].Item2, this.radius);
                mesh = CombineMeshes(mesh, segment);
            }

            return mesh;
        }

        /// <summary>
        /// A helper method to combine two meshes into a single mesh.
        /// </summary>
        /// <param name="mesh1">The first mesh.</param>
        /// <param name="mesh2">The second mesh.</param>
        /// <returns>The combined mesh.</returns>
        private Mesh CombineMeshes(Mesh mesh1, Mesh mesh2)
        {
            var vertices = mesh1.vertices.ToList();
            int n = vertices.Count();
            var triangles = mesh1.triangles.ToList();
            vertices = mesh1.vertices.Concat(mesh2.vertices).ToList();
            foreach (int triangle in mesh2.triangles)
            {
                triangles.Add(triangle + n);
            }

            Mesh combined = new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            return combined;
        }

        /// <summary>
        /// Creates a Circle Mesh around a center with the given radius.
        /// The mesh consists of a given number of triangles. 
        /// Use a higher number of segments to create a smoother circle.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="numSegments">The number of segments of the mesh.</param>
        /// <returns>The circle mesh. </returns>
        private Mesh CircleMesh(Vector3 center, float radius, int numSegments)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            vertices.Add(center);

            for (int i = 0; i < numSegments; i++)
            {
                float angle = i * (360f / numSegments);
                vertices.Add(center + (Quaternion.Euler(0, angle, 0) * Vector3.right * radius));

                triangles.Add(0);
                triangles.Add(i + 1);
                if (i + 2 == numSegments + 1)
                {
                    triangles.Add(1);
                }
                else
                {
                    triangles.Add(i + 2);
                }
            }

            Mesh circle = new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            return circle;
        }

        /// <summary>
        /// Creates a (rotated)rectangle between two points with a width of two times the given radius.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>The Mesh segment.</returns>
        private Mesh SegmentMesh(Vector3 start, Vector3 end, float radius)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            var fdir = end - start;
            var fnormal = Vector3.Cross(fdir, Vector3.up).normalized * radius;
            vertices.Add(start + fnormal);
            vertices.Add(start - fnormal);

            var ldir = start - end;
            var lnormal = Vector3.Cross(ldir, Vector3.up).normalized * radius;
            vertices.Add(end + lnormal);
            vertices.Add(end - lnormal);

            triangles.Add(0);
            triangles.Add(2);
            triangles.Add(1);

            triangles.Add(0);
            triangles.Add(3);
            triangles.Add(2);

            Mesh segment = new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            return segment;
        }
    }
}