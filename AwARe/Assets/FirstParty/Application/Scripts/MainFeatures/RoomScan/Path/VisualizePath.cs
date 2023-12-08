using UnityEngine;
//using Palmmedia.ReportGenerator.Core.Parser.Analysis;

namespace AwARe.RoomScan.Path
{
    public class VisualizePath : MonoBehaviour
    {
        [Header("Path")]
        /// <value>
        /// The data of the path to visualize
        /// </value>
        [SerializeField] private PathData pathData;

        [Header("Settings")]
        /// <value>
        /// The width of the line that visualizes the skeleton of the path.
        /// </value>
        [SerializeField] private float lineWidth = 1.0f;
        /// <value>
        /// The number of segments to use for the corners. Use a higher value to create smoother edges.
        /// </value>
        [SerializeField] private int numSegments = 6;
        /// <value>
        /// The material used to render the path Mesh.
        /// </value>
        [SerializeField] private Material pathMeshMaterial;
        /// <value>
        /// The material used to render the skeleton line.
        /// </value>
        [SerializeField] private Material pathLineMaterial;

        /// <summary>
        /// Set the path to visualize.
        /// </summary>
        public void SetPath(PathData path)
        {
            this.pathData = path;
        }

        /// <summary>
        /// Visualize the path. It will clear previous visualisations.
        /// </summary>
        public void Visualize()
        {
            // Clear previous visualisation
            foreach (Transform child in this.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Create path mesh
            var pathMesh = new GameObject("PathMesh");
            var renderer = pathMesh.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            renderer.material = pathMeshMaterial;
            var filter = pathMesh.AddComponent(typeof(MeshFilter)) as MeshFilter;
            filter.mesh = pathData.CreateMesh(this.numSegments);
            pathMesh.transform.parent = this.transform;

            // Create path lines 
            foreach(var points in pathData.edges)
            {
                var segment = new GameObject("PathSegment");
                var line = segment.AddComponent(typeof(LineRenderer)) as LineRenderer;
                line.positionCount = 2;
                line.SetPositions(new Vector3[] { points.Item1, points.Item2 });
                line.material = pathLineMaterial;
                line.startWidth = lineWidth;
                line.endWidth = lineWidth;
                // Make sure line is rendered on top of mesh
                line.transform.Translate(Vector3.up * 0.01f);
                segment.transform.parent = this.transform;
            }
        }
    }
}
