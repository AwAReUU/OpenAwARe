using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PolygonMesh : MonoBehaviour
{
    [SerializeField] private List<Vector3> polygon = new();

    void Start()
    {
        this.CreateMesh();
        this.SetHeight(1f);
    }

    public void SetHeight(float height)
    {

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
            vertices.Add(this.polygon[i] + new Vector3(0f, 1f, 0f));

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
