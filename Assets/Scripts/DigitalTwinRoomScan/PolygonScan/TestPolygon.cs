using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPolygon : Polygon
{
    private void Start()
    {
        line = this.transform.GetChild(0).GetComponent<LineRenderer>();

        this.points.Add(new Vector3(0, -1.5f, 0));
        this.points.Add(new Vector3(1f, -1.5f, 0));
        this.points.Add(new Vector3(1f, -1.5f, 0.8f));
        this.points.Add(new Vector3(0f, -1.5f, 1));
        this.points.Add(new Vector3(0, -1.5f, 0));
        UpdateLine();
    }
    private void UpdateLine()
    {
        line.positionCount = this.points.Count;
        line.SetPositions(this.points.ToArray());
    }

    protected override void Update()
    {
        
    }
}