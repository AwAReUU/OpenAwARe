// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;

using AwARe.InterScenes.Objects;
using AwARe.Data.Logic;
using AwARe.Data.Objects;
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

        [SerializeField] private GameObject createBtn;
        [SerializeField] private GameObject resetBtn;
        [SerializeField] private GameObject applyBtn;
        [SerializeField] private GameObject confirmBtn;
        [SerializeField] private GameObject endBtn;
        [SerializeField] private GameObject slider;
        [SerializeField] private GameObject pointerObj;

        private bool scanning = false;
// TODO TEMP        [SerializeField] private GameObject pathVisualiser;

        /// <summary>
        /// All UI components of the polygon scan.
        /// </summary>
        List<GameObject> UIObjects;

        /// <value>A Room represented by the polygons.</value>
        public Room Room { get; private set; }

        /// <value>The polygon currently being drawn.</value>
        public Polygon CurrentPolygon { get; private set; }

        void Start()
        {
            CurrentPolygon = new Polygon();

            Room = new Room();

            // Use the code below to use the test room
            //Room = new TestRoom();
            //polygonDrawer.DrawRoomPolygons(Room);

            UIObjects = new()
            {
                createBtn, resetBtn, confirmBtn, slider, applyBtn, endBtn,
                pointerObj, scanner.gameObject, polygonMesh.gameObject   // TODO TEMP   , pathVisualiser
            };

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

            for(int i = 0; i < CurrentPolygon.Points.Count; i++)
            {
                Debug.Log(CurrentPolygon.Points[i]);
            }

            GenerateAndDrawPath();
        }

        public void GenerateAndDrawPath()
        {
            PathGenerator startstate = new();

            bool useTestPol = true;
            
            List<Vector3> points = new()
            {
                new Vector3(0.3290718f, 0, -1.92463f),
                new Vector3(-0.5819738f, 0, -2.569284f),
                new Vector3(3.357841f, 0, -3.58952f),
                new Vector3(3.824386f, 0, -2.0016f),
            };

            Polygon testPolygon = new Polygon(points);

            PathData path;
            if (useTestPol)
            {
                path = startstate.GeneratePath(testPolygon, Room.NegativePolygons);
                polygonDrawer.DrawPolygon(testPolygon);
            }
            else
            { 
                path = startstate.GeneratePath(Room.PositivePolygon, Room.NegativePolygons);
            }

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
        public void OnHeightSliderChanged(float height)
        {
            this.polygonMesh.SetHeight(height);
        }

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
            foreach (GameObject obj in UIObjects)
            {
                obj.SetActive(false);
            }

            foreach (GameObject obj in GetStateObjects(toState))
            {
                obj.SetActive(true);
            }

            // if the new state is scanning, set scanning to true, otherwise to false
            scanning = toState == State.Scanning;

            polygonDrawer.ScanningPolygon = null;
        }

        /// <summary>
        /// Gets the UI objects that need to be present in this state.
        /// </summary>
        /// <param name="state">A state of the scanning process.</param>
        /// <returns>A list of UI objects that need to be present in the given state.</returns>
        List<GameObject> GetStateObjects(State state)
        {
            List<GameObject> objects = new();
            switch (state)
            {
                case State.Default:
                    objects.Add(createBtn);
                    break;
                case State.Scanning:
                    objects.Add(applyBtn);
                    objects.Add(resetBtn);
                    objects.Add(scanner.gameObject);
                    objects.Add(pointerObj);
// TODO TEMP                    objects.Add(pathVisualiser);
                break;
                case State.SettingHeight:
                    objects.Add(confirmBtn);
                    objects.Add(slider);
                    objects.Add(polygonMesh.gameObject);
// TODO TEMP                    objects.Add(pathVisualiser);
                break;
                case State.Saving:
                    objects.Add(createBtn);
                    objects.Add(endBtn);
                    break;

            }
            return objects;
        }

        /// <summary>
        /// The different states within the polygon scanning process.
        /// </summary>
        enum State
        {
            Default,
            Scanning,
            SettingHeight,
            Saving
        }
    }
}