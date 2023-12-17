// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Objects;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Polygons.Logic;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private RoomManager roomManager;
        [SerializeField] private PolygonDrawer polygonDrawer;

        [SerializeField] private Polygon polygon;

        [SerializeField] private PolygonUI ui;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Transform sceneCanvas;

        private State currentState;

        /// <value>A Room represented by the polygons.</value>
        public Room Room { get => roomManager.Room; private set => roomManager.Room = value; }

        /// <value>The Polygon currently being drawn.</value>
        private Polygon activePolygon;
        private Mesher polygonMesh;
        private Liner polygonLine;

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

        // TODO: Make Object.Room for active visibility.
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
            Room = new Room();
            StartScanning();
        }

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
            polygonMesh = activePolygon.GetComponent<Mesher>();
            polygonLine = activePolygon.GetComponent<Liner>();

            polygonMesh.UpdateMesh();
            polygonLine.UpdateLine();
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
            polygonMesh.UpdateMesh();
        }

        public void OnSaveButtonClick()
        {
            Storage.Get().ActiveRoom = Room.Data;
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
            currentState = toState;
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
            bool mesh = false, drawer = false;
            switch (state)
            {
                case State.Scanning:
                    drawer = true;
                    break;
                case State.SettingHeight:
                    mesh = true;
                    break;
            }

            // Set actual activity
        }
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