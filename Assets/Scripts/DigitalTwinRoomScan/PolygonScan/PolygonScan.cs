using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PolygonScan : MonoBehaviour
{
    [SerializeField] private GameObject pointer;

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
        RaycastHit hitData;
        Physics.Raycast(ray, out hitData);

        if (hitData.transform.gameObject.name.Contains("ARPlane") &&
                hitData.normal.x == 0f && hitData.normal.y == 1f && hitData.normal.z == 0f)
        {
            // Check if hitpoint is on a horizontal ar plane.
            pointer.transform.position = hitData.point;
        }
    }
}
