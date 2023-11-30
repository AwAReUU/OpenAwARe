using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PolygonManager : MonoBehaviour
{
    [SerializeField] private PolygonDrawer polygonDrawer;
    [SerializeField] private PolygonMesh polygonMesh;
    [SerializeField] private PolygonScan scanner;

    [SerializeField] private GameObject createBtn;
    [SerializeField] private GameObject resetBtn;
    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject confirmBtn;
    [SerializeField] private GameObject endBtn;
    [SerializeField] private GameObject slider;
    [SerializeField] private GameObject pointerObj;
    [SerializeField] private GameObject pathVisualiser;

    List<GameObject> UIObjects;

    private Polygon positivePolygon;
    private List<Polygon> negativePolygons;

    public Polygon CurrentPolygon { get; private set; } // the polygon currently being drawn

    public void Start()
    {
        CurrentPolygon = new Polygon();

        //positivePolygon = new TestPolygon();
        //negativePolygons = new TestPolygonList();
        negativePolygons = new();

        //polygonDrawer.DrawAllPolygons(positivePolygon, negativePolygons);

        UIObjects = new()
        {
            createBtn, resetBtn, confirmBtn, slider, applyBtn, endBtn, pointerObj, scanner.gameObject, polygonMesh.gameObject
        };

        SwitchToState(State.Default);
    }

    public void OnApplyButtonClick()
    {
        polygonDrawer.Apply();
        SwitchToState(State.SettingHeight);
        polygonMesh.SetPolygon(CurrentPolygon.GetPoints());

        if (positivePolygon == null)
        {
            positivePolygon = CurrentPolygon;
            polygonDrawer.DrawPolygon(CurrentPolygon);
        }
        else
        {
            negativePolygons.Add(CurrentPolygon);
            polygonDrawer.DrawPolygon(CurrentPolygon, true);
        }

        //do path gen en visualise here

        //create a copy with swapped y and z
        Polygon poscopy = new();
        List<Vector3> pospolypoints = positivePolygon.GetPointsList();
        float pathheight = 0;
        for (int i = 0; i < pospolypoints.Count; i++)
        {
            poscopy.AddPoint(new Vector3(pospolypoints[i].x, pospolypoints[i].z));
            pathheight = pospolypoints[i].y;
        }
        List<Polygon> negcopies = new();
        for(int i = 0; i < negativePolygons.Count; i++)
        {
            Polygon negcopy = new();
            List<Vector3> negpolypoints = negativePolygons[i].GetPointsList();
            for(int j = 0; j < negpolypoints.Count; j++)
            {
                negcopy.AddPoint(new Vector3(negpolypoints[j].x, negpolypoints[j].z));
            }
            negcopies.Add(negcopy);
        }

        AltStartState startstate = new();
        PathData path = startstate.GetStartState(poscopy, negcopies);

        //swap y and z back
        PathData pathcopy = new();
        for(int i = 0; i < path.points.Count; i++)
        {
            pathcopy.points.Add(new Vector3(path.points[i].x, 0, path.points[i].y));
        }
        for(int i = 0; i < path.edges.Count; i++)
        {
            pathcopy.edges.Add((new Vector3(path.edges[i].Item1.x, pathheight, path.edges[i].Item1.y),
                                new Vector3(path.edges[i].Item2.x, pathheight, path.edges[i].Item2.y)));
        }

        VisualizePath visualizer = (VisualizePath)pathVisualiser.GetComponent("VisualizePath");
        visualizer.SetPath(pathcopy);
        visualizer.Visualize();
    }

    public void ResetPolygon()
    {
        CurrentPolygon = new();
        polygonDrawer.Reset();
        SwitchToState(State.Scanning);
    }

    public void Confirm()
    {
        // TODO: set room height
        SwitchToState(State.Saving);
    }

    // public void EndPolyScan()
    // {
    //     this.slider.SetActive(false);
    //     this.endBtn.SetActive(false);
    // }

    public void OnSlider(System.Single height)
    {
        this.polygonMesh.SetHeight(height);
    }

    public Polygon GetPolygon()
    {
        return this.positivePolygon;
    }

    public List<Polygon> GetNegPolygons()
    {
        return this.negativePolygons;
    }

    private void SwitchToState(State toState)
    {
        foreach(GameObject obj in UIObjects)
        {
            obj.SetActive(false);
        }

        foreach(GameObject obj in GetStateObjects(toState))
        {
            obj.SetActive(true);
        }
    }

    enum State
    {
        Default,
        Scanning,
        SettingHeight,
        Saving
    }

    List<GameObject> GetStateObjects(State state)
    {
        List<GameObject> objects = new();
        switch(state)
        {
            case State.Default:
                objects.Add(createBtn);
                break;
            case State.Scanning:
                objects.Add(applyBtn);
                objects.Add(resetBtn);
                objects.Add(scanner.gameObject);
                objects.Add(pointerObj);
                break;
            case State.SettingHeight:
                objects.Add(confirmBtn);
                objects.Add(slider);
                objects.Add(polygonMesh.gameObject);
                break;
            case State.Saving:
                objects.Add(createBtn);
                objects.Add(endBtn);
                break;

        }
        return objects;
    }
}
