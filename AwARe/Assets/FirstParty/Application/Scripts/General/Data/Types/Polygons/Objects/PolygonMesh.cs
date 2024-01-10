// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// The mesh representing the polygon; used to set the height.
    /// </summary>
    public class PolygonMesh : MonoBehaviour
    {
        private float height = 1f;
        private List<Vector3> polygon = new();
        private Renderer renderer;

        void Start()
        {
            CreateMesh();
            SetHeight(1f);
            renderer = GetComponent<Renderer>();
        }


        public void SetPolygonColor(Color color)
        {
            renderer.material.color = color;
        }
        /// <summary>
        /// Set the height of the mesh.
        /// </summary>
        /// <param name="height">The height the mesh should be set to.</param>
        public void SetHeight(float height)
        {
            this.height = height;
            CreateMesh();
        }

        public void ApplyColorToMesh()
        {
            // Check if the mesh renderer is assigned
            if (renderer != null)
            {
                renderer.material = new Material(renderer.material);
            }
            else
            {
                Debug.LogError("Renderer not assigned in PolygonMesh.");
            }
        }
        /// <summary>
        /// Sets the local polygon to the given list of points.
        /// </summary>
        /// <param name="points">An array of points representing the polygon.</param>
        public void SetPolygon(Vector3[] points)
        {
            polygon = points.ToList();
            CreateMesh();
        }

        private void CreateMesh()
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();

            int n = polygon.Count * 2;
            for (int i = 0; i < polygon.Count; i++)
            {
                vertices.Add(polygon[i]);
                vertices.Add(polygon[i] + new Vector3(0f, height, 0f));

                int j = i * 2;
                triangles.Add(j);
                triangles.Add((j + 1) % n);
                triangles.Add((j + 3) % n);
                triangles.Add(j);
                triangles.Add((j + 3) % n);
                triangles.Add((j + 2) % n);
            }

            Mesh mesh = new()
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            gameObject.GetComponent<MeshFilter>().mesh = mesh;
        }

    }
}