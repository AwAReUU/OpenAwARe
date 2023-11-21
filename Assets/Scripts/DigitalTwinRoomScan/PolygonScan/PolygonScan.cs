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
    [SerializeField] private PolygonDrawer polygonDrawer;

    private GameObject trackables;

    // Start is called before the first frame update
    void Start()
    {
        trackables = GameObject.Find("Trackables");
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Physics.Raycast(ray, out RaycastHit hitData);

        if (hitData.transform != null && hitData.transform.gameObject.name.Contains("ARPlane") &&
                hitData.normal.x == 0f && hitData.normal.y == 1f && hitData.normal.z == 0f)
        {
            // Check if hitpoint is on a horizontal ar plane.
            pointer.transform.position = hitData.point;

            polygonDrawer.SetPointer(pointer.transform.position);
        }
        else
        {
            // Check if plane and ray are not parrallel.
            if (ray.direction.y != 0)
            {
                float l = (-1.5f - ray.origin.y) / ray.direction.y;
                // Check if ray is not reversed
                if (l > 0f)
                {
                    pointer.transform.position = ray.origin + ray.direction * l;

                    polygonDrawer.SetPointer(pointer.transform.position);
                }
            }
        }


        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            polygonDrawer.AddPoint();
        }
    }

}
