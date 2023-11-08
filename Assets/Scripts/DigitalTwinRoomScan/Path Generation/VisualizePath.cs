using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class VisualizePath : MonoBehaviour
{
    [Header("Polygon")]
    [SerializeField] private Mesh polygonMesh;

    [Header("Path")]
    [SerializeField] private List<Vector3> points;
    [SerializeField] private float radius;

    [Header("Settings")]
    [SerializeField] private int numSegments = 6;


    private GameObject polygon;
    private GameObject path;
    private GameObject pathInline;
    private GameObject pathOutline;

    public void Start()
    {
        this.polygon = this.transform.GetChild(0).gameObject;
        this.path = this.transform.GetChild(1).gameObject;
        this.pathInline = this.path.transform.GetChild(1).gameObject;
        this.pathOutline = this.path.transform.GetChild(0).gameObject;

        this.Visualize();
    }

    public void Visualize()
    {
        // * Visualize polygon mesh
        this.polygon.GetComponent<MeshFilter>().mesh = this.polygonMesh;

        // * Visualize path
        var inline = this.pathInline.GetComponent<LineRenderer>();
        var outline = this.pathOutline.GetComponent<MeshFilter>();

        inline.positionCount = this.points.Count;
        inline.SetPositions(this.points.ToArray());

        outline.mesh = this.PathMesh(this.points.ToArray());
    }

    private Mesh PathMesh(Vector3[] points)
    {
        Mesh mesh = new();

        for (int i = 0; i < points.Length; i++)
        {
            Mesh circle = this.CircleMesh(points[i], this.radius);
            mesh = CombineMeshes(mesh, circle);

            if (i == points.Length - 1)
                continue;
            Mesh segment = this.SegmentMesh(points[i], points[i + 1], this.radius);
            mesh = CombineMeshes(mesh, segment);
        }

        return mesh;
    }

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

    private Mesh CircleMesh(Vector3 center, float radius)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        vertices.Add(center);

        for (int i = 0; i < this.numSegments; i++)
        {
            float angle = i * (360f / this.numSegments);
            vertices.Add(center + (Quaternion.Euler(0, angle, 0) * Vector3.right * radius));

            triangles.Add(0);
            triangles.Add(i + 1);
            if (i + 2 == this.numSegments + 1)
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
