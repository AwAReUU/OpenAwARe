using UnityEngine;
//using Palmmedia.ReportGenerator.Core.Parser.Analysis;

namespace AwARe.RoomScan.Path
{
    public class VisualizePath : MonoBehaviour
    {
        [Header("Path")]
        [SerializeField] private PathData pathData;

        [Header("Settings")]
        [SerializeField] private float lineWidth = 1.0f;
        [SerializeField] private int numSegments = 6;
        [SerializeField] private Material pathMeshMaterial;
        [SerializeField] private Material pathLineMaterial;

        public void SetPath(PathData path)
        {
            this.pathData = path;
        }

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
