using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

public class VisualizePath : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private PathData pathData;

    [Header("Settings")]
    [SerializeField] private float lineWidth = 1.0f;
    [SerializeField] private int numSegments = 6;
    [SerializeField] private Material pathMeshMaterial;
    [SerializeField] private Material pathLineMaterial;


    public void Start()
    {
        Polygon mockpositive = new Polygon();
        mockpositive.AddPoint(new Vector3(0, 1));
        mockpositive.AddPoint(new Vector3(5, 1));
        mockpositive.AddPoint(new Vector3(5, -4));
        mockpositive.AddPoint(new Vector3(10, -4));
        mockpositive.AddPoint(new Vector3(10, 6));
        mockpositive.AddPoint(new Vector3(0, 6));

        Polygon mocknegative1 = new Polygon();
        mocknegative1.AddPoint(new Vector3(1, 5));
        mocknegative1.AddPoint(new Vector3(3, 5));
        mocknegative1.AddPoint(new Vector3(3, 2));
        mocknegative1.AddPoint(new Vector3(1, 2));

        Polygon mocknegative2 = new Polygon();
        mocknegative2.AddPoint(new Vector3(7.5f, -1.5f));
        mocknegative2.AddPoint(new Vector3(9, -1.5f));
        mocknegative2.AddPoint(new Vector3(9, -3));
        mocknegative2.AddPoint(new Vector3(7.5f, -3));

        List<Polygon> mocknegatives = new();

        AltStartState bean = new();
        this.pathData = bean.GetStartState(mockpositive, mocknegatives);

        //turn y into z cords, see if visual better?
        for(int i = 0; i < this.pathData.edges.Count; i++)
        {
            pathData.edges[i] = (new Vector3(pathData.edges[i].Item1.x, 0, pathData.edges[i].Item1.y), new Vector3(pathData.edges[i].Item2.x, 0, pathData.edges[i].Item2.y));
        }



        this.Visualize();
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
        //foreach (var points in pathData.Segments()) //old
        foreach(var points in pathData.edges)         //new
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
