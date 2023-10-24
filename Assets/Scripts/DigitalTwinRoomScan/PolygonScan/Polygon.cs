using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    [SerializeField] private readonly GameObject pointerObj;
    [SerializeField] private readonly GameObject scannerObj;

    private Vector3 pointer = Vector3.zero;
    private List<Vector3> points = new List<Vector3>();

    private LineRenderer line;
    private LineRenderer temp_line;
    private LineRenderer close_line;

    // Start is called before the first frame update
    void Start()
    {
        this.line = this.transform.GetChild(0).GetComponent<LineRenderer>();
        this.temp_line = this.transform.GetChild(1).GetComponent<LineRenderer>();
        this.close_line = this.transform.GetChild(2).GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCloseLine();
        UpdateTempLine();
    }

    public void Reset()
    {
        pointerObj.SetActive(true);
        scannerObj.SetActive(true);
        this.pointer = Vector3.zero;
        this.points = new List<Vector3>();
        this.UpdateLine();
        this.UpdateTempLine();
        this.UpdateCloseLine();
    }

    public void Apply()
    {
        this.line.loop = true;
        this.UpdateLine();
        this.UpdateTempLine();
        this.UpdateCloseLine();
        pointerObj.SetActive(false);
        scannerObj.SetActive(false);
    }

    public void SetPointer(Vector3 pointer)
    {
        this.pointer = pointer;
    }

    public void AddPoint()
    {
        this.points.Add(this.pointer);

        UpdateLine();
    }

    public void RemoveLast()
    {
        if (this.points.Count > 0)
        {
            this.points.RemoveAt(this.points.Count - 1);
        }
        UpdateLine();
    }

    private void UpdateLine()
    {
        this.line.positionCount = this.points.Count;
        this.line.SetPositions(this.points.ToArray());
    }

    private void UpdateTempLine()
    {
        if (this.points.Count > 0)
        {
            this.temp_line.positionCount = 2;
            Vector3[] points = { this.points[^1], this.pointer };
            this.temp_line.SetPositions(points);
        }
        else
        {

            this.temp_line.positionCount = 0;
            Vector3[] empty = { };
            this.temp_line.SetPositions(empty);
        }
    }

    private void UpdateCloseLine()
    {
        if (this.points.Count > 1)
        {
            this.close_line.positionCount = 2;
            Vector3[] points = { this.points[0], this.pointer };
            this.close_line.SetPositions(points);
        }
        else
        {
            this.close_line.positionCount = 0;
        }
    }
}
