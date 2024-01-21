// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Linq;

using AYellowpaper;

using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// A mesh constructor class for the referred polygon.
    /// </summary>
    [Serializable]
    public class PolygonMesherLogic : MonoBehaviour, IMesherLogic
    {
        /// <summary>
        /// The polygon to represent.
        /// </summary>
        public InterfaceReference<IDataHolder<Polygon>> polygon;
        
        /// <summary>
        /// Gets the data of the polygon.
        /// </summary>
        /// <value>The data of the polygon.</value>
        private Polygon Data => polygon.Value.Data;

        /// <summary>
        /// Adds this component to a given GameObject and initializes the components members.
        /// </summary>
        /// <param name="gameObject">The GameObject this component is added to.</param>
        /// <param name="polygon">A polygon.</param>
        /// <returns>The added component.</returns>
        public static PolygonMesherLogic AddComponentTo(GameObject gameObject, InterfaceReference<IDataHolder<Polygon>> polygon)
        {
            var logic = gameObject.AddComponent<PolygonMesherLogic>();
            logic.polygon = polygon;
            return logic;
        }

        /// <inheritdoc/>
        public Mesh Mesh =>
            ConstructMesh(Data);

        /// <summary>
        /// Constructs a mesh representing the given Polygon. <br/>
        /// If the Polygon is set to null, then return an empty mesh.
        /// </summary>
        /// <param name="polygon">The Polygon to construct a mesh from.</param>
        /// <returns>A mesh representing the given Polygon.</returns>
        private Mesh ConstructMesh(Polygon polygon = null) =>
            // Handle null input.
            polygon == null ? new() : ConstructMesh_Body(polygon);

        /// <summary>
        /// Constructs a mesh representing the given Polygon.
        /// </summary>
        /// <param name="polygon">The Polygon to construct a mesh from.</param>
        /// <returns>A mesh representing the given Polygon.</returns>
        private Mesh ConstructMesh_Body(Polygon polygon)
        {
            // Get the data.
            var points = polygon.points;
            var height = polygon.height;

            // Construct the vertices.
            int n = points.Count;
            Vector3 min = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Vector3 max = new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            Vector3[] vertices = new Vector3[n * 2 + 1];
            for (int i = 0, j = 0; i < n; i++)
            {
                vertices[j++] = points[i];
                vertices[j++] = points[i] + new Vector3(0f, height, 0f);

                // Keep track of outer points
                min.x = Math.Min(min.x, points[i].x);
                min.y = Math.Min(min.y, points[i].y);
                min.z = Math.Min(min.z, points[i].z);
                max.x = Math.Max(max.x, points[i].x);
                max.y = Math.Max(max.y, points[i].y);
                max.z = Math.Max(max.z, points[i].z);
            }
            // Add middle point
            vertices[n * 2] = new Vector3((min.x + max.x) / 2, (min.y + max.y) / 2 + height, (min.z + max.z) / 2);

            // Construct the faces/triangles.
            n = vertices.Length - 1;
            int[] triangles = new int[n * 12];
            for (int i = 0, j = 0; i < n; i += 2)
            {
                // Walls
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

                // Top
                triangles[j++] = n;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = (i + 1) % n;

                triangles[j++] = n;
                triangles[j++] = (i + 1) % n;
                triangles[j++] = (i + 3) % n;

                triangles[j++] = n;
                triangles[j++] = (i + 5) % n;
                triangles[j++] = (i + 3) % n;

                triangles[j++] = n;
                triangles[j++] = (i + 3) % n;
                triangles[j++] = (i + 5) % n;
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