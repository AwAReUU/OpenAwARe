using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation.VisualScripting;

public class PolygonDrawer : MonoBehaviour
{
    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject pointerObj;

    private Vector3 pointer = Vector3.zero;

    [SerializeField] private GameObject lineObject;
    [SerializeField] private LineRenderer temp_line;
    [SerializeField] private LineRenderer close_line;

    private LineRenderer line;
    public Polygon polygon { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        line = lineObject.GetComponent<LineRenderer>();
        polygon = new Polygon();
        applyBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCloseLine();
        UpdateTempLine();
    }

    public void Reset()
    {
        this.line.loop = false;
        applyBtn.SetActive(false);
        this.pointer = Vector3.zero;
        this.polygon = new();
        this.UpdateLine(line, polygon);
        this.UpdateTempLine();
        this.UpdateCloseLine();
    }

    public void Apply()
    {
        applyBtn.SetActive(false);
        this.line.loop = true;
        this.UpdateLine(line, polygon);
        this.UpdateTempLine();
        this.UpdateCloseLine();
    }

    public void SetPointer(Vector3 pointer)
    {
        this.pointer = pointer;
    }

    public void AddPoint()
    {
        applyBtn.SetActive(true);
        polygon.AddPoint(this.pointer);
        this.pointerObj.SetActive(false);

        UpdateLine(line, polygon);
    }

    public void RemoveLast()
    {
        polygon.RemoveLastPoint();
        UpdateLine(line, polygon);
    }

    public void UpdateLine(LineRenderer line, Polygon polygon)
    {
        line.positionCount = polygon.AmountOfPoints();
        line.SetPositions(polygon.GetPoints());
    }

    private void UpdateTempLine()
    {
        if (polygon.AmountOfPoints() > 0)
        {
            this.temp_line.positionCount = 2;
            Vector3[] points = { polygon.GetPoints()[^1], this.pointer };
            this.temp_line.SetPositions(points);
        }
        else
        {
            this.temp_line.positionCount = 0;
            // this.temp_line.SetPositions(new Vector3[] { });
        }
    }

    private void UpdateCloseLine()
    {
        if (polygon.AmountOfPoints() > 1)
        {
            this.close_line.positionCount = 2;
            Vector3[] points = { polygon.GetFirstPoint(), this.pointer };
            this.close_line.SetPositions(points);
        }
        else
        {
            this.close_line.positionCount = 0;
        }
    }

    public void DrawNewPolygon(Polygon newPolygon, bool isNegPolygon = false)
    {
        GameObject newLineObject = Instantiate(lineObject, this.gameObject.transform);
        LineRenderer newLine = newLineObject.GetComponent<LineRenderer>();
        if(isNegPolygon)
        {
            newLine.startColor = Color.red;
            newLine.endColor = Color.red;
        }
        newPolygon.AddPoint(newPolygon.GetFirstPoint());
        UpdateLine(newLine, newPolygon);
    }
}