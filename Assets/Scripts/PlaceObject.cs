using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class PlaceObject : MonoBehaviour
{
    public GameObject[] prefabs;
    public int prefabIndex;

    private ARRaycastManager aRRaycastManager;
    private ARPlaneManager aRPlaneManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private EventSystem eventSystem;

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRPlaneManager = GetComponent<ARPlaneManager>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    //* Interact using touch screen (mobile)
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        //TODO Hij interact nog steeds door de UI heen dus fix dit :)
        //* Don't interact with world when clicking on a UI element
        if(eventSystem.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

        Interact(finger.currentTouch.screenPosition);
    }

    //* Interact using mouse button (pc)
    private void Update()
    {
        //* Don't interact with world when clicking on a UI element
        if (eventSystem.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0))
            Interact(Input.mousePosition);
    }

    private void Interact(Vector2 screenPoint)
    {
        if (aRRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
        {
            foreach (ARRaycastHit hit in hits)
            {
                Pose pose = hit.pose;
                GameObject obj = Instantiate(prefabs[prefabIndex], pose.position, pose.rotation);

                if (aRPlaneManager.GetPlane(hit.trackableId).alignment == PlaneAlignment.HorizontalUp)
                {
                    Vector3 position = obj.transform.position;
                    Vector3 cameraPosition = Camera.main.transform.position;
                    Vector3 direction = cameraPosition - position;
                    Vector3 targetRotationEuler = Quaternion.LookRotation(direction).eulerAngles;
                    Vector3 scaledEuler = Vector3.Scale(targetRotationEuler, obj.transform.up.normalized);
                    Quaternion targetRotation = Quaternion.Euler(scaledEuler);
                    obj.transform.rotation *= targetRotation;
                }
            }
        }
    }
}
