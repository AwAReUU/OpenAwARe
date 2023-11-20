using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonManager : MonoBehaviour
{
    [SerializeField] private PolygonDrawer polygonDrawer;
    [SerializeField] private PolygonMesh polygonMesh;
    [SerializeField] private PolygonScan scanner;
    [SerializeField] private GameObject pointerObj;

    [SerializeField] private Polygon positivePolygon;
    [SerializeField] private List<Polygon> negativePolygons;

    [SerializeField] private GameObject resetBtn;
    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject confirmBtn;
    [SerializeField] private GameObject slider;

    public Polygon CurrentPolygon { get; private set; }

    public void Start()
    {
        positivePolygon = new TestPolygon();
        negativePolygons = new TestPolygonList();

        polygonDrawer.DrawNewPolygon(positivePolygon);
        foreach(Polygon p in negativePolygons)
        {
            polygonDrawer.DrawNewPolygon(p, true);
        }
    }

    public void Apply()
    {
        this.polygonDrawer.Apply();
        this.applyBtn.SetActive(false);
        this.confirmBtn.SetActive(true);
        this.scanner.gameObject.SetActive(false);
        this.pointerObj.SetActive(false);
    }

    public void Reset()
    {
        this.polygonDrawer.Reset();
        this.scanner.gameObject.SetActive(true);
        this.pointerObj.SetActive(true);
    }

    public void Confirm()
    {
        this.applyBtn.SetActive(false);
        this.resetBtn.SetActive(false);
        this.confirmBtn.SetActive(false);
        this.polygonMesh.gameObject.SetActive(true);
        this.polygonMesh.SetPolygon(this.polygonDrawer.polygon.GetPoints());
        this.slider.SetActive(true);
        positivePolygon = polygonDrawer.polygon;
    }

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
}
