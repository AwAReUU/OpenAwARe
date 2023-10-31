using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

// Kizi: I changed the ARplanemanager to accept a pointcloud manager for now instead to geenrate the pointcloud for the boundingbox
public enum InputStates { All, Place, Interact }
[RequireComponent(typeof(ARRaycastManager), typeof(ARPointCloudManager))]

public class InputController : MonoBehaviour
{
    private ARRaycastManager aRRaycastManager;
    private ARPointCloudManager aRCloudManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ObjectCreationManager objectCreationManager;
    private InteractObjectHandler interactObjectHandler;
    private Camera arCamera;
    public InputStates inputState { get; private set; }

    public void SetInputState(InputStates state)
    {
        inputState = state;
    }
    
    public void SetInputState(int index)
    {
        inputState = (InputStates)index;
    }

    private void Awake()
    {
        aRRaycastManager = GetComponent<ARRaycastManager>();
        aRCloudManager = GetComponent<ARPointCloudManager>();
        objectCreationManager = GetComponent<ObjectCreationManager>();
        interactObjectHandler = GetComponent<InteractObjectHandler>();
        arCamera = FindObjectOfType<Camera>();
        SetInputState(InputStates.All);
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        //EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    private void OnDisable()
    {
        //EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
    }

    //? I disabled all Fingerdown references as they are bugged, see IsPointerOverUIObject(). 
    //? Inputs are detected through mouse in Update() which for some reason also detects finger inputs.
    //* Interact using touch screen (mobile)
    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        HandleInput(finger.currentTouch.screenPosition);
    }

    //* Interact using mouse button (pc)
    //? Update should only be enabled within the editor, but it's currently enabled for mobile as well
    //? Due to the bug explained at FingerDown() and IsPointerOverUIObject()
    //#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleInput(Input.mousePosition);
    }
    //#endif

    private bool IsPointerOverUIObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        //check touch
        //TODO Fix this input bug
        //! This is bugged; it's like a touch delayed. 
        //! E.g. when I press a UI button the first time it still sends a raycast behind the button. 
        //! After the first touch it works correctly, but then the first (and only the first) touch
        //! in the worldscene is not detected, after which world inputs work correctly again.
        // if (EnhancedTouch.Touch.activeTouches.Count > 0)
        // {
        //     if (EventSystem.current.IsPointerOverGameObject(EnhancedTouch.Touch.activeTouches[0].touchId))
        //     {
        //         return true;
        //     }
        // }

        return false;
    }

    private void HandleInput(Vector2 screenPoint)
    {
        if (IsPointerOverUIObject())
            return;

        //* Interact with existing object in world
        if (inputState == InputStates.Interact || inputState == InputStates.All)
        {
            Ray ray = arCamera.ScreenPointToRay(screenPoint);
            RaycastHit hitObject;
            if (Physics.Raycast(ray, out hitObject, 20f, LayerMask.GetMask("Ingredient")))
            {
                //interactObjectHandler.ColorObject(hitObject.transform.gameObject);
                interactObjectHandler.ToggleDataWindow(hitObject.transform.gameObject);
                return;
            }
        }

        //* Place new object in world
        if (inputState == InputStates.Place || inputState == InputStates.All)
        {
            if (aRRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
            {
                // foreach (ARRaycastHit hit in hits)
                //     createObjectHandler.CreateObject(hit);
                if (hits.Count > 0)
                    objectCreationManager.TryPlaceObjectOnTouch(hits[0]);
            }
        }
    }
}