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

    List<GameObject> UIObjects;

    private Polygon positivePolygon;
    private List<Polygon> negativePolygons;

    public Polygon CurrentPolygon { get; private set; } // the polygon currently being drawn

    public void Start()
    {
        CurrentPolygon = new Polygon();

        positivePolygon = new TestPolygon();
        negativePolygons = new TestPolygonList();

        polygonDrawer.DrawAllPolygons(positivePolygon, negativePolygons);

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
