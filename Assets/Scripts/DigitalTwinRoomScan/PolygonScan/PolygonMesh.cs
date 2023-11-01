using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PolygonMesh : MonoBehaviour
{
    private float height = 1f;
    private List<Vector3> polygon = new();

    void Start()
    {
        this.CreateMesh();
        this.SetHeight(1f);
    }

    public void SetHeight(float height)
    {
        this.height = height;
        this.CreateMesh();
    }

    public void SetPolygon(Vector3[] points)
    {
        this.polygon = points.ToList();
        this.CreateMesh();
    }

    private void CreateMesh()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();

        int n = this.polygon.Count * 2;
        for (int i = 0; i < this.polygon.Count; i++)
        {
            vertices.Add(this.polygon[i]);
            vertices.Add(this.polygon[i] + new Vector3(0f, this.height, 0f));

            int j = i * 2;
            triangles.Add(j);
            triangles.Add((j + 1) % n);
            triangles.Add((j + 3) % n);
            triangles.Add(j);
            triangles.Add((j + 3) % n);
            triangles.Add((j + 2) % n);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        this.gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

}
