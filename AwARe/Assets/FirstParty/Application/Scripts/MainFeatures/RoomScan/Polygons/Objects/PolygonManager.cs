// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Linq;

using AwARe.Data.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Objects;
using AwARe.UI;
using AwARe.UI.Objects;
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

        public bool IsActive =>
            CurrentState is State.Drawing or State.SettingHeight;

        /// <summary>
        /// Gets the currently active room.
        /// </summary>
        /// <value>
        /// A Room represented by the polygons.
        /// </value>
        public Room Room { get => manager.Room; private set => manager.Room = value; }

        void Start() =>
            SwitchToState(State.Default);

        /// <summary>
        /// Add the given polygon to the room.
        /// </summary>
        /// <param name="polygon">A polygon.</param>
        public void AddPolygon(Polygon polygon)
        {
            if (polygon.Data.points.Count <= 0) return; // Don't add an empty polygon

            if (Room.positivePolygon == null)
                Room.positivePolygon = polygon;
            else
                Room.negativePolygons.Add(polygon);
            polygon.transform.SetParent(Room.transform);
        }

        /// <summary>
        /// Starts a new Polygon scan.
        /// </summary>
        public void StartScanning()
        {
            polygonDrawer.StartDrawing();
            SwitchToState(State.Drawing);
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
            Data.Logic.Room roomData = new();
            Room.Data = roomData;
            polygonDrawer.Reset();
            StartScanning();
        }

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss()
        {
            if(CurrentState == State.Drawing)
            {
                if (!polygonDrawer.pointer.Value.FoundFirstPlane && !Application.isEditor)
                    Debug.LogError("No plane found yet. Please try again.");
                else
                {
                    polygonDrawer.AddPoint();

                    polygonDrawer.pointer.Value.LockPlane = true;
                }
            }
        }

        /// <summary>
        /// Called on apply button click; adds and draws the current Polygon.
        /// </summary>
        public void OnApplyButtonClick()
        {
            polygonDrawer.FinishDrawing(out Data.Logic.Polygon data);

            if (data.points.Count > 0)
            {
                activePolygon = Instantiate(polygon, transform);
                activePolygon.gameObject.SetActive(true);
                activePolygon.Data = data;

                activePolygonMesh = activePolygon.GetComponent<Mesher>();
                activePolygonMesh.UpdateMesh();
                activePolygonLine = activePolygon.GetComponent<Liner>();
                activePolygonLine.UpdateLine();

                SwitchToState(State.SettingHeight);
            }
            else
            {
                SwitchToState(State.Done);
            }
        }

        /// <summary>
        /// Called on confirm button click; sets the height of the Polygon.
        /// </summary>
        public void OnConfirmButtonClick()
        {
            AddPolygon(activePolygon);
            SwitchToState(State.Done);

            // Set color for the finished polygon
            Color polygonColor = Color.green; // You can choose any color
            Mesh mesh = activePolygonMesh.meshFilter.mesh;
            mesh.colors = mesh.vertices.Select(_ => polygonColor).ToArray();
            activePolygonMesh.meshFilter.mesh = mesh;
            activePolygonMesh.UpdateMesh();
        }

        /// <summary>
        /// Called on changing the slider; sets the height of the Polygon mesh.
        /// </summary>
        /// <param name="height">height the slider is currently at.</param>
        public void OnHeightSliderChanged(float height)
        {
            activePolygon.Data.height = height;
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