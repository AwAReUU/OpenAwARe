using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PathData
{
    public List<Vector3> points;
    public List<(Vector3, Vector3)> edges;

    public float radius;

    // i have altered the edges list so that is is basically this, but also cleaner.
    // public IEnumerable<(Vector3, Vector3)> Segments()
    // {
    //     // //old
    //     // for (int i = 0; i < edges.Count; i += 2)
    //     // {
    //     //     yield return new Tuple<Vector3, Vector3>(points[edges[i]], points[edges[i + 1]]);
    //     // }

    //     //new
    //     for(int i = 0; i < edges.Count; i++)
    //     {
    //         yield return edges[i];
    //     }
    // }

    public Mesh CreateMesh(int numSegments)
    {

        Mesh mesh = new();

        for (int i = 0; i < points.Count; i++)
        {
            Mesh circle = CircleMesh(points[i], this.radius, numSegments);
            mesh = CombineMeshes(mesh, circle);
        }

        // //old
        // for (int i = 0; i < edges.Count; i += 2)
        // {
        //     Mesh segment = this.SegmentMesh(points[edges[i]], points[edges[i + 1]], this.radius);
        //     mesh = CombineMeshes(mesh, segment);
        // }

        //new
        for(int i = 0; i < edges.Count; i++)
        {
            Mesh segment = this.SegmentMesh(edges[i].Item1, edges[i].Item2, this.radius);
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