using AwARe.DataTypes;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

namespace AwARe.InputControl
{
    public enum InputState { All, Place, Interact }

    public class Ref<T>
    {
        public Ref(T value) =>
            Value = value;

        public T Value { get; set; }

        public static implicit operator T(Ref<T> r) => r.Value;
    }

    public class Get<T>
    {
        public Get(System.Func<T> getter) =>
            this.Getter = getter;

        public System.Func<T> Getter { get; set; }

        public T Value { get => Getter(); }

        public static implicit operator Get<T>(System.Func<T> a) => new(a);
        public static implicit operator T(Get<T> g) => g.Value;
    }

    internal interface IInputHandler
    {
        public bool HandleInput(Vector2 screenPoint);
    }

    [RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
    public class InputController : MonoBehaviour, IInputHandler
    {
        public InputState inputState;

        private IInputHandler inputHandler;

        private Vector2? firstFingerPosition;
        private Vector2? mousePosition;

        public void SetInputState(InputState state) =>
            inputState = state;

        public void SetInputState(int index) =>
            inputState = (InputState)index;

        private void Awake()
        {
            inputState = InputState.All;

            var aRRaycastManager = GetComponent<ARRaycastManager>();
            var aRPlaneManager = GetComponent<ARPlaneManager>();
            var objectCreationManager = GetComponent<ObjectCreationManager>();
            var interactObjectHandler = GetComponent<InteractObjectHandler>();
            var arCamera = FindObjectOfType<Camera>();
            var getInputState = (Get<InputState>)(() => inputState);

            InputHandlerFactory factory = new InputHandlerFactory();
            inputHandler = factory.CreateRegular(arCamera, interactObjectHandler, aRRaycastManager, objectCreationManager, getInputState);
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
            if (finger.index == 0) 
                firstFingerPosition = finger.currentTouch.screenPosition;
        }

        private void MouseButtonDown() =>
            mousePosition = Input.GetMouseButtonDown(0) ? Input.mousePosition : null;

        //* Interact using mouse button (pc)
        //? Update should only be enabled within the editor, but it's currently enabled for mobile as well
        //? Due to the bug explained at FingerDown() and IsPointerOverUIObject()
        //#if UNITY_EDITOR
        private void Update()
        {
            // Update input
            MouseButtonDown();

            // Check input
            Vector2? screenPosition = firstFingerPosition ?? mousePosition;
            if (screenPosition.HasValue) HandleInput(screenPosition.Value);
        }
        //#endif

        public bool HandleInput(Vector2 screenPoint) =>
            inputHandler.HandleInput(screenPoint);
    }

    public class InputHandler_Regular : IInputHandler
    {
        private InputHandler_PressUI presser;
        private InputHandler_SelectObject selecter;
        private InputHandler_PlaceObject placer;
        private Get<InputState> inputState;

        public InputHandler_Regular(InputHandler_PressUI presser, InputHandler_SelectObject selecter, InputHandler_PlaceObject placer, Get<InputState> inputState)
        {
            this.presser = presser; this.selecter = selecter; this.placer = placer; this.inputState = inputState;
        }

        public bool HandleInput(Vector2 screenPoint)
        {
            //* Interact with existing UI in world
            if (presser.HandleInput(screenPoint)) return true;

            //* Interact with existing object in world
            if ((inputState == InputState.Interact || inputState == InputState.All))
                if (selecter.HandleInput(screenPoint)) return true;

            //* Place new object in world
            if (inputState == InputState.Place || inputState == InputState.All)
                if (placer.HandleInput(screenPoint)) return true;

            return false;
        }
    }

    public class InputHandler_VoxelMap : IInputHandler
    {
        private InputHandler_PressUI presser;

        public InputHandler_VoxelMap(InputHandler_PressUI presser)
        {
            this.presser = presser;
        }

        public bool HandleInput(Vector2 screenPoint)
        {
            //* Interact with existing UI in world
            if (presser.HandleInput(screenPoint)) return true;

            return false;
        }
    }

    public class InputHandler_SelectObject : IInputHandler
    {
        private Camera arCamera;
        private InteractObjectHandler interactObjectHandler;

        public InputHandler_SelectObject(Camera arCamera, InteractObjectHandler interactObjectHandler)
        {
            this.arCamera = arCamera; this.interactObjectHandler = interactObjectHandler;
        }

        public bool HandleInput(Vector2 screenPoint)
        {
            //* Interact with existing object in world
            Ray ray = arCamera.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out RaycastHit hitObject, 20f, LayerMask.GetMask("Ingredient")))
            {
                //interactObjectHandler.ColorObject(hitObject.transform.gameObject);
                interactObjectHandler.ToggleDataWindow(hitObject.transform.gameObject);
                return true;
            }
            return false;
        }
    }

    public class InputHandler_PlaceObject : IInputHandler
    {
        private ARRaycastManager aRRaycastManager;
        private ObjectCreationManager objectCreationManager;

        private List<ARRaycastHit> hits = new List<ARRaycastHit>();

        public InputHandler_PlaceObject(ARRaycastManager aRRaycastManager, ObjectCreationManager objectCreationManager)
        {
            this.aRRaycastManager = aRRaycastManager; this.objectCreationManager = objectCreationManager;
        }

        public bool HandleInput(Vector2 screenPoint)
        {
            //* Place new object in world
            if (aRRaycastManager.Raycast(screenPoint, hits, TrackableType.PlaneWithinPolygon))
            {
                // foreach (ARRaycastHit hit in hits)
                //     createObjectHandler.CreateObject(hit);
                if (hits.Count > 0)
                {
                    objectCreationManager.TryPlaceObjectOnTouch(hits[0]);
                    return true;
                }
            }
            return false;
        }
    }

    public class InputHandler_PressUI : IInputHandler
    {
        public InputHandler_PressUI() { }

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

        public bool HandleInput(Vector2 screenPoint) =>
            IsPointerOverUIObject();
    }

    public class InputHandlerFactory
    {
        public InputHandlerFactory() { }

        public InputHandler_PressUI CreatePressUI() =>
            new();

        public InputHandler_SelectObject CreateSelectObject(Camera arCamera, InteractObjectHandler interactObjectHandler) =>
            new(arCamera, interactObjectHandler);

        public InputHandler_PlaceObject CreatePlaceObject(ARRaycastManager aRRaycastManager, ObjectCreationManager objectCreationManager) =>
            new(aRRaycastManager, objectCreationManager);


        public InputHandler_Regular CreateRegular(Camera arCamera, InteractObjectHandler interactObjectHandler, ARRaycastManager aRRaycastManager, ObjectCreationManager objectCreationManager, Get<InputState> inputState) =>
            new(CreatePressUI(), CreateSelectObject(arCamera, interactObjectHandler), CreatePlaceObject(aRRaycastManager, objectCreationManager), inputState);

        public InputHandler_VoxelMap CreateVoxelMap() =>
            new(new());

        public InputHandlers CreateInputHandlers(Camera arCamera, InteractObjectHandler interactObjectHandler, ARRaycastManager aRRaycastManager, ObjectCreationManager objectCreationManager, Get<InputState> inputState)
        {
            var pressUI = new InputHandler_PressUI();
            var selectObject = new InputHandler_SelectObject(arCamera, interactObjectHandler);
            var placeObject = new InputHandler_PlaceObject(aRRaycastManager, objectCreationManager);
            var regular = new InputHandler_Regular(pressUI, selectObject, placeObject, inputState);
            var voxelmap = new InputHandler_VoxelMap(pressUI);
            return new(pressUI, selectObject, placeObject, regular, voxelmap);
        }
    }

    public class InputHandlers
    {
        public InputHandlers(
            InputHandler_PressUI pressUI, InputHandler_SelectObject selectObject, InputHandler_PlaceObject placeObject,
            InputHandler_Regular regular, InputHandler_VoxelMap voxelmap
            )
        {
            PressUI = pressUI; SelectObject = selectObject; PlaceObject = placeObject;
            Regular = regular; VoxelMap = voxelmap;
        }

        public InputHandler_PressUI PressUI { get; private set; }
        public InputHandler_SelectObject SelectObject { get; private set; }
        public InputHandler_PlaceObject PlaceObject { get; private set; }
        public InputHandler_Regular Regular { get; private set; }
        public InputHandler_VoxelMap VoxelMap { get; private set; }
    }
}