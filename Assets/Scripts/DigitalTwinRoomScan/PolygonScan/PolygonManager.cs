using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonManager : MonoBehaviour
{
    [SerializeField] private GameObject polygon;
    [SerializeField] private GameObject polygonMesh;
    [SerializeField] private PolygonScan scanner;
    [SerializeField] private GameObject pointerObj;

    [SerializeField] private GameObject resetBtn;
    [SerializeField] private GameObject applyBtn;
    [SerializeField] private GameObject confirmBtn;
    [SerializeField] private GameObject slider;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Apply()
    {
        this.polygon.GetComponent<Polygon>().Apply();
        this.applyBtn.SetActive(false);
        this.confirmBtn.SetActive(true);
        this.scanner.enabled = false;
        this.pointerObj.SetActive(false);
    }

    public void Reset()
    {
        this.polygon.GetComponent<Polygon>().Reset();
        this.scanner.enabled = true;
        this.pointerObj.SetActive(true);
    }

    public void Confirm()
    {
        this.applyBtn.SetActive(false);
        this.resetBtn.SetActive(false);
        this.confirmBtn.SetActive(false);
        this.polygonMesh.SetActive(true);
        this.polygonMesh.GetComponent<PolygonMesh>().SetPolygon(this.polygon.GetComponent<Polygon>().GetPoints());
        this.slider.SetActive(true);
    }

    public void OnSlider(System.Single height)
    {
        this.polygonMesh.GetComponent<PolygonMesh>().SetHeight(height);
    }
}
