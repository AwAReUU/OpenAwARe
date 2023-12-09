// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Polygons.Logic;
using UnityEngine;
using Storage = AwARe.InterScenes.Objects.Storage;

namespace AwARe.RoomScan.Polygons.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the polygon scanning.
    /// </summary>
    public class PolygonManager : MonoBehaviour
    {
        [SerializeField] private PolygonDrawer polygonDrawer;
        [SerializeField] private PolygonMesh polygonMesh;
        [SerializeField] private PolygonScan scanner;
        [SerializeField] private VisualizePath pathVisualizer;

        [SerializeField] private PolygonUI ui;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Transform sceneCanvas;

        private bool scanning = false;

        /// <value>A Room represented by the polygons.</value>
        public Room Room { get; private set; }

        /// <value>The polygon currently being drawn.</value>
        public Polygon CurrentPolygon { get; private set; }

        private void Awake()
        {
            if (canvas != null)
            {
                ui.transform.parent = sceneCanvas;
                Destroy(canvas);
            }
        }

        void Start()
        {
            CurrentPolygon = new Polygon();
            Room = new Room();

            // Use the code below to use the test room
            //Room = new TestRoom();
            //polygonDrawer.DrawRoomPolygons(Room);

            SwitchToState(State.Default);
        }

        void Update()
        {
            if (scanning)
                polygonDrawer.ScanningPolygon = CurrentPolygon;
        }

        /// <summary>
        /// Starts a new polygon scan.
        /// </summary>
        public void StartScanning()
        {
            CurrentPolygon = new();
            polygonDrawer.Reset();
            SwitchToState(State.Scanning);
        }

        /// <summary>
        /// Called on create button click; Starts a new polygon scan.
        /// </summary>
        public void OnCreateButtonClick() =>
            StartScanning();


        /// <summary>
        /// Called on reset button click; Clears the room and starts a new polygon scan.
        /// </summary>
        public void OnResetButtonClick()
        {
            Room = new Room();
            StartScanning();
        }


        /// <summary>
        /// Called on apply button click; adds and draws the current polygon.
        /// </summary>
        public void OnApplyButtonClick()
        {
            polygonDrawer.ClearScanningLines();
            SwitchToState(State.SettingHeight);
            polygonMesh.SetPolygon(CurrentPolygon.GetPoints());

            polygonDrawer.DrawPolygon(CurrentPolygon, !Room.PositivePolygon.IsEmptyPolygon());
            Room.AddPolygon(CurrentPolygon);
            //GenerateAndDrawPath();
        }

        public void GenerateAndDrawPath()
        {
            StartState startstate = new();
            PathData path = startstate.GetStartState(Room.PositivePolygon, Room.NegativePolygons);

            VisualizePath visualizer = (VisualizePath)pathVisualizer.GetComponent("VisualizePath");
            visualizer.SetPath(path);
            visualizer.Visualize();
        }

        /// <summary>
        /// Called on confirm button click; sets the height of the polygon.
        /// </summary>
        public void OnConfirmButtonClick()
        {
            // TODO: set room height
            SwitchToState(State.Saving);
        }

        /// <summary>
        /// Called on changing the slider; sets the height of the polygon mesh.
        /// </summary>
        /// <param name="height">Height the slider is currently at.</param>
        public void OnHeightSliderChanged(float height) { this.polygonMesh.SetHeight(height); }

        public void OnSaveButtonClick()
        {
            Storage.Get().ActiveRoom = Room;
            SceneSwitcher.Get().LoadScene("Home");
        }

        /// <summary>
        /// Sets all UIObjects to inactive, then activates all UIObjects of this state.
        /// </summary>
        /// <param name="toState">Which state the UI should switch to.</param>
        private void SwitchToState(State toState)
        {
            // Set activity
            SetActive(toState);

            // if the new state is scanning, set scanning to true, otherwise to false
            scanning = toState == State.Scanning;
            polygonDrawer.ScanningPolygon = null;
        }

        /// <summary>
        /// Sets activity of components.
        /// </summary>
        /// <param name="toState">Current/new state.</param>
        public void SetActive(State state)
        {
            // Set UI activity
            ui.SetActive(state);

            // Set direct component activity
            bool scan = false, mesh = false;
            switch (state)
            {
                case State.Scanning:
                    scan = true;
                    break;
                case State.SettingHeight:
                    mesh = true;
                    break;
            }
            scanner.gameObject.SetActive(scan);
            polygonMesh.gameObject.SetActive(mesh);
        }
    }

    /// <summary>
    /// The different states within the polygon scanning process.
    /// </summary>
    public enum State
    {
        Default,
        Scanning,
        SettingHeight,
        Saving
    }
}