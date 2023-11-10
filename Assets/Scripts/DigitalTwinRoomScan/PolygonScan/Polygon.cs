using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Polygon : MonoBehaviour
{
    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject pointerObj;

    private Vector3 pointer = Vector3.zero;
    protected List<Vector3> points = new();
    protected LineRenderer line;
    private LineRenderer temp_line;
    private LineRenderer close_line;

    // Start is called before the first frame update
    void Start()
    {
        this.line = this.transform.GetChild(0).GetComponent<LineRenderer>();
        this.temp_line = this.transform.GetChild(1).GetComponent<LineRenderer>();
        this.close_line = this.transform.GetChild(2).GetComponent<LineRenderer>();
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
        this.points = new List<Vector3>();
        this.UpdateLine();
        this.UpdateTempLine();
        this.UpdateCloseLine();
    }

    public void Apply()
    {
        applyBtn.SetActive(false);
        this.line.loop = true;
        this.UpdateLine();
        this.UpdateTempLine();
        this.UpdateCloseLine();
    }

    public void SetPointer(Vector3 pointer)
    {
        this.pointer = pointer;
    }

    public Vector3[] GetPoints()
    {
        return this.points.ToArray();
    }

    public List<Vector3> GetPointsList()
    {
        return this.points;
    }

    public void AddPoint()
    {
        applyBtn.SetActive(true);
        this.points.Add(this.pointer);
        this.pointerObj.SetActive(false);

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
            // this.temp_line.SetPositions(new Vector3[] { });
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
