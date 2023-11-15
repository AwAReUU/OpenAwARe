using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.XR.ARFoundation;

public class PolygonScan : MonoBehaviour
{
    [SerializeField] private GameObject pointer;
    [SerializeField] private GameObject polygon;

    private GameObject trackables;

    // Start is called before the first frame update
    public void Start()
    {
        trackables = GameObject.Find("Trackables");
    }

    // Update is called once per frame
    public void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(ray, out RaycastHit hitData);

        if (hitData.transform != null && hitData.transform.gameObject.name.Contains("ARPlane") &&
                hitData.normal.x == 0f && hitData.normal.y == 1f && hitData.normal.z == 0f)
        {
            // Check if hitpoint is on a horizontal ar plane.
            pointer.transform.position = hitData.point;

            polygon.GetComponent<Polygon>().SetPointer(pointer.transform.position);
        }
        else
        {
            // Check if plane and ray are not parrallel.
            if (ray.direction.y != 0)
            {
                float l = (pointer.transform.position.y - ray.origin.y) / ray.direction.y;
                // Check if ray is not reversed
                if (l > 0f)
                {
                    pointer.transform.position = ray.origin + ray.direction * l;

                    polygon.GetComponent<Polygon>().SetPointer(pointer.transform.position);
                }
            }
        }


        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            polygon.GetComponent<Polygon>().AddPoint();
        }
    }

}
