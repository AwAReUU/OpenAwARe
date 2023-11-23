using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARFoundation.VisualScripting;

public class PolygonDrawer : MonoBehaviour
{
    [SerializeField] private PolygonManager polygonManager;

    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject pointerObj;


    [SerializeField] private GameObject lineObject;
    [SerializeField] private LineRenderer tempLine;
    [SerializeField] private LineRenderer closeLine;

    private LineRenderer line;

    private Vector3 pointer = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        line = lineObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCloseLine();
        UpdateTempLine();
    }

    public void Reset()
    {
        pointer = Vector3.zero;
        UpdateLine(line, polygonManager.CurrentPolygon);
        tempLine.gameObject.SetActive(true);
        closeLine.gameObject.SetActive(true);
    }

    public void Apply()
    {
        UpdateLine(line, polygonManager.CurrentPolygon);
        ClearScanningLines();
    }

    public void SetPointer(Vector3 pointer)
    {
        this.pointer = pointer;
    }

    public void AddPoint()
    {
        Polygon polygon = polygonManager.CurrentPolygon;
        polygon.AddPoint(pointer);
        pointerObj.SetActive(false);

        UpdateLine(line, polygon);
    }

    public void RemoveLast()
    {
        Polygon polygon = polygonManager.CurrentPolygon;
        polygon.RemoveLastPoint();
        UpdateLine(line, polygon);
    }
    public void ClearScanningLines()
    {
        tempLine.gameObject.SetActive(false);
        closeLine.gameObject.SetActive(false);
    }

    public void UpdateLine(LineRenderer line, Polygon polygon)
    {
        line.positionCount = polygon.AmountOfPoints();
        line.SetPositions(polygon.GetPoints());
    }

    private void UpdateTempLine()
    {
        Polygon polygon = polygonManager.CurrentPolygon;
        if (polygon.AmountOfPoints() > 0)
        {
            tempLine.positionCount = 2;
            Vector3[] points = { polygon.GetPoints()[^1], this.pointer };
            tempLine.SetPositions(points);
        }
        else
        {
            tempLine.positionCount = 0;
            // this.tempLine.SetPositions(new Vector3[] { });
        }
    }

    private void UpdateCloseLine()
    {
        Polygon polygon = polygonManager.CurrentPolygon;
        if (polygon.AmountOfPoints() > 1)
        {
            closeLine.positionCount = 2;
            Vector3[] points = { polygon.GetFirstPoint(), pointer };
            closeLine.SetPositions(points);
        }
        else
        {
            closeLine.positionCount = 0;
        }
    }

    public void DrawAllPolygons(Polygon posPolygon, List<Polygon> negPolygons)
    {
        DrawPolygon(posPolygon);
        foreach (Polygon p in negPolygons)
        {
            DrawPolygon(p, true);
        }
    }

    public void DrawPolygon(Polygon newPolygon, bool isNegPolygon = false)
    {
        GameObject newLineObject = Instantiate(lineObject, transform);
        LineRenderer newLine = newLineObject.GetComponent<LineRenderer>();
        if(isNegPolygon)
        {
            newLine.startColor = Color.red;
            newLine.endColor = Color.red;
        }
        newLine.loop = true;
        //newPolygon.AddPoint(newPolygon.GetFirstPoint());
        UpdateLine(newLine, newPolygon);
    }
}