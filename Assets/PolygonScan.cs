using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PolygonScan : MonoBehaviour
{
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private GameObject pointer;

    private List<ARRaycastHit> raycastHits = new();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (raycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), raycastHits))
        {
            Debug.Log(raycastHits[0].trackable);
            Debug.Log(raycastHits.Count);
            pointer.transform.position = raycastHits[0].pose.position;
        }
    }
}
