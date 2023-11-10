using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPolygon : Polygon
{
    private List<Vector3> points = new();
    private LineRenderer line;
    private void Start()
    {
        this.line = this.transform.GetChild(0).GetComponent<LineRenderer>();

        this.points.Add(new Vector3(0, -1.5f, 0));
        this.points.Add(new Vector3(1f, -1.5f, 0));
        this.points.Add(new Vector3(1f, -1.5f, 0.8f));
        this.points.Add(new Vector3(0f, -1.5f, 1));
        this.points.Add(new Vector3(0, -1.5f, 0));
        UpdateLine();
    }
    new private void UpdateLine()
    {
        this.line.positionCount = this.points.Count;
        this.line.SetPositions(this.points.ToArray());
    }
}