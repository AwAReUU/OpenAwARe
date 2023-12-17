// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.Data.Objects
{
    [Serializable]
    public class PolygonMesherLogic : MonoBehaviour, IMesher
    {
        [SerializeField] private Polygon polygon;
        private Logic.Polygon Data => polygon.Data;

        public PolygonMesherLogic(Polygon polygon)
        {
            this.polygon = polygon;
        }

        /// <inheritdoc/>
        public Mesh Mesh =>
            ComputeMesh(Data);

        /// <summary>
        /// Compute a mesh representing the given Polygon. <br/>
        /// If the Polygon is set to null, then return an empty mesh.
        /// </summary>
        /// <param name="polygon">The Polygon to compute a mesh from.</param>
        /// <returns>A mesh representing the given Polygon.</returns>
        private Mesh ComputeMesh(Logic.Polygon polygon = null) =>
            // Handle null input.
            polygon == null ? new() : ComputeMesh_Body(polygon);

        /// <summary>
        /// Compute a mesh representing the given Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to compute a mesh from.</param>
        /// <returns>A mesh representing the given Polygon.</returns>
        private Mesh ComputeMesh_Body(Logic.Polygon polygon)
        {
            Debug.Log("ComputeMesh_Body");
            // Get the data.
            var points = polygon.Points;
            var height = polygon.Height;

            // Compute the vertices.
            int n = points.Count;
            Vector3[] vertices = new Vector3[n * 2];
            for (int i = 0, j = 0; i < n; i++)
            {
                vertices[j++] = points[i];
                vertices[j++] = points[i] + new Vector3(0f, height, 0f);
            }

            // Compute the faces/triangles.
            n = vertices.Length;
            int[] triangles = new int[n * 6];
            for (int i = 0, j = 0; i < n; i += 2)
            {
                triangles[j++] = i;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = (i + 1) % n;

                triangles[j++] = (i + 1) % n;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = i;
                
                triangles[j++] = i;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = (i + 2) % n;
                
                triangles[j++] = (i + 2) % n;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = i;
            }

            // Return the mesh
            return new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                name = "Polygon"
            };
        }
    }
}