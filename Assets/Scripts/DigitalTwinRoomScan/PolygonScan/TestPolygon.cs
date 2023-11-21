using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPolygon : Polygon
{
    public TestPolygon()
    {
        points = new()
        {
            new Vector3(-5,-1.5f,-5),
            new Vector3(5,-1.5f,-4),
            new Vector3(5, -1.5f, 6),
            new Vector3(-4,-1.5f,6)
        };
    }
}

public class TestPolygonList : List<Polygon>
{
    public TestPolygonList()
    {
        List<Vector3> pol1Points = new()
        {
            new Vector3(0,-1.5f,0),
            new Vector3(1,-1.5f,0),
            new Vector3(1,-1.5f,2),
            new Vector3(0,-1.5f,2)
        };

        List<Vector3> pol2Points = new()
        {
            new Vector3(-3,-1.5f,-3),
            new Vector3(-2,-1.5f,-3),
            new Vector3(-2,-1.5f,-1),
            new Vector3(-2.3f,-1.5f,-1.2f),
            new Vector3(-2.8f,-1.5f,-1.3f)
        };

        this.Add(CreatePolygon(pol1Points));
        this.Add(CreatePolygon(pol2Points));
    }

    /// <summary>
    /// Creates a polygon and fills its points list with the given points
    /// </summary>
    /// <param name="points">The points of which the polygon consists</param>
    /// <returns>The polygon consisting of the given points</returns>
    Polygon CreatePolygon(List<Vector3> points)
    {
        Polygon polygon = new();
        foreach (Vector3 point in points)
        {
            polygon.AddPoint(point);
        }
        return polygon;
    }
}