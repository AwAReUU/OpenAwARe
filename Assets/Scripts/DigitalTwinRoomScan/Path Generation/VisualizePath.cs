using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizePath : MonoBehaviour
{
    [Header("Polygon")]
    [SerializeField] private Mesh polygonMesh;
    [SerializeField] private Material polygonMaterial;
    [Header("Path")]
    [SerializeField] private List<Vector3> points;
    [SerializeField] private float radius;


}
