// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Objects;
using UnityEngine;

using Polygon = AwARe.Data.Objects.Polygon;
using Room = AwARe.Data.Objects.Room;
using Storage = AwARe.InterScenes.Objects.Storage;

namespace AwARe.RoomScan.Polygons.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class PolygonManager : MonoBehaviour
    {
        // The upper management
        [SerializeField] private RoomManager roomManager;

        // Objects to control
        [SerializeField] private PolygonDrawer polygonDrawer;

        // The UI
        [SerializeField] private PolygonUI ui;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Transform sceneCanvas;

        // Object templates
        [SerializeField] private Polygon polygon;

        // Tracking data
        private State currentState;
        private Polygon activePolygon;
        private Mesher activePolygonMesh;
        private Liner activePolygonLine;

        /// <summary>
        /// Gets the currently active room.
        /// </summary>
        /// <value>
        /// A Room represented by the polygons.
        /// </value>
        public Room Room { get => roomManager.Room; private set => roomManager.Room = value; }
        
        /// <summary>
        /// Gets the current position of the pointer.
        /// </summary>
        /// <value>
        /// The current position of the pointer.
        /// </value>
        public Vector3 PointedAt => ui.PointedAt;

        private void Awake()
        {
            if (canvas != null)
            {
                ui.transform.SetParent(sceneCanvas, false);
                Destroy(canvas);
            }
        }

        void Start()
        {
            SwitchToState(State.Default);
        }

        /// <summary>
        /// Add the given polygon to the room.
        /// </summary>
        /// <param name="polygon">A polygon.</param>
        public void AddPolygon(Polygon polygon)
        {
            if (Room.PositivePolygon == null)
                Room.PositivePolygon = polygon;
            Room.NegativePolygons.Add(polygon);
        }

        /// <summary>
        /// Starts a new Polygon scan.
        /// </summary>
        public void StartScanning()
        {
            polygonDrawer.StartDrawing();
            SwitchToState(State.Scanning);
        }

        /// <summary>
        /// Called on create button click; Starts a new Polygon scan.
        /// </summary>
        public void OnCreateButtonClick() =>
            StartScanning();


        /// <summary>
        /// Called on reset button click; Clears the room and starts a new Polygon scan.
        /// </summary>
        public void OnResetButtonClick()
        {
            Room = new();
            StartScanning();
        }

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss()
        {
            if(currentState == State.Scanning)
                polygonDrawer.AddPoint();
        }

        /// <summary>
        /// Called on apply button click; adds and draws the current Polygon.
        /// </summary>
        public void OnApplyButtonClick()
        {
            polygonDrawer.FinishDrawing(out Data.Logic.Polygon data);
            activePolygon = Instantiate(polygon, transform);
            activePolygon.gameObject.SetActive(true);
            activePolygon.Data = data;
            activePolygonMesh = activePolygon.GetComponent<Mesher>();
            activePolygonLine = activePolygon.GetComponent<Liner>();

            activePolygonMesh.UpdateMesh();
            activePolygonLine.UpdateLine();
            SwitchToState(State.SettingHeight);
        }

        /// <summary>
        /// Called on confirm button click; sets the height of the Polygon.
        /// </summary>
        public void OnConfirmButtonClick()
        {
            AddPolygon(activePolygon);
            SwitchToState(State.Saving);
        }

        /// <summary>
        /// Called on changing the slider; sets the height of the Polygon mesh.
        /// </summary>
        /// <param name="height">Height the slider is currently at.</param>
        public void OnHeightSliderChanged(float height)
        {
            activePolygon.Data.Height = height;
            activePolygonMesh.UpdateMesh();
        }

        /// <summary>
        /// Called on save button click; Stores the current room and switches to the home screen.
        /// </summary>
        public void OnSaveButtonClick()
        {
            Storage.Get().ActiveRoom = Room.Data;
            SceneSwitcher.Get().LoadScene("Home");
        }

        /// <summary>
        /// Sets all UIObjects to inactive, then activates all UIObjects of this state.
        /// </summary>
        /// <param name="state">Which state the UI should switch to.</param>
        private void SwitchToState(State state)
        {
            // Set activity
            SetActive(state);
            currentState = state;
        }

        /// <summary>
        /// Sets activity of components.
        /// </summary>
        /// <param name="state">Current/new state.</param>
        public void SetActive(State state) =>
            // Set UI activity
            ui.SetActive(state);
    }

    /// <summary>
    /// The different states within the Polygon scanning process.
    /// </summary>
    public enum State
    {
        Default,
        Scanning,
        SettingHeight,
        Saving
    }
}