using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPolygon
{
    public abstract void UpdateLine();
    public Vector3[] GetPoints();

    public List<Vector3> GetPointsList();
}
