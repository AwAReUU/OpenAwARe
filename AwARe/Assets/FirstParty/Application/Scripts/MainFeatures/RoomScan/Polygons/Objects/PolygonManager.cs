// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Objects;
using UnityEngine;

namespace AwARe.RoomScan.Polygons.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class PolygonManager : MonoBehaviour
    {
        // The upper management
        [SerializeField] private RoomManager manager;

        // Objects to control
        [SerializeField] private PolygonDrawer polygonDrawer;

        // Object templates
        [SerializeField] private Polygon polygon;

        // Tracking data
        private Polygon activePolygon;
        private Mesher activePolygonMesh;
        private Liner activePolygonLine;

        /// <summary>
        /// Gets the current state of the polygon scanner.
        /// </summary>
        /// <value>
        /// The current state of the polygon scanner.
        /// </value>
        public State CurrentState { get; private set; }

        /// <summary>
        /// Gets the currently active room.
        /// </summary>
        /// <value>
        /// A Room represented by the polygons.
        /// </value>
        public Room Room { get => manager.Room; private set => manager.Room = value; }
        
        /// <summary>
        /// Gets the current position of the pointer.
        /// </summary>
        /// <value>
        /// The current position of the pointer.
        /// </value>
        public Vector3 PointedAt => manager.PointedAt;

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
            if (Room.positivePolygon == null)
                Room.positivePolygon = polygon;
            else
                Room.negativePolygons.Add(polygon);
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
            if(CurrentState == State.Scanning)
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
        /// Sets all Objects activities to match new state.
        /// </summary>
        /// <param name="state">The new state switched to.</param>
        private void SwitchToState(State state)
        {
            CurrentState = state;
            manager.SetActive();
        }
    }
}