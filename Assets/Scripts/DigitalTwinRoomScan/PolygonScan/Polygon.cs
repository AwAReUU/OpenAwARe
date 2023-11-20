using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    protected List<Vector3> points;

    public Polygon()
    {
        points = new List<Vector3>();
    }

    public void AddPoint(Vector3 point)
    {
        points.Add(point);
    }

    public void RemoveLastPoint()
    {
        if (points.Count > 0)
        {
            this.points.RemoveAt(this.points.Count - 1);
        }
    }

    public Vector3[] GetPoints()
    {
        return this.points.ToArray();
    }

    public List<Vector3> GetPointsList()
    {
        return this.points;
    }

    public int AmountOfPoints()
    {
        return points.Count;
    }
    public Vector3 GetFirstPoint()
    {
        return (points[0]);
    }
}
