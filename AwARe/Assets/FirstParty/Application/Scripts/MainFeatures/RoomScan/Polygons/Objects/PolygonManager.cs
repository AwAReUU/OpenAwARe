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
        /// Adds a point if the current state is drawing.
        /// </summary>
        public void TryAddPoint()
        {
            if (CurrentState == State.Drawing)
                polygonDrawer.AddPoint();
        }

        /// <summary>
        /// Finishes drawing the current Polygon.
        /// </summary>
        public void FinishPolygon()
        {
            if (polygonDrawer.Polygon.points.Count > 0)
            {
                polygonDrawer.FinishDrawing(out Data.Logic.Polygon data);

                activePolygon = Instantiate(polygon, transform);
                activePolygon.gameObject.SetActive(true);
                activePolygon.Data = data;

                activePolygonMesh = activePolygon.GetComponent<Mesher>();
                activePolygonMesh.UpdateMesh();
                activePolygonLine = activePolygon.GetComponent<Liner>();
                activePolygonLine.UpdateLine();
            }
            else
            {
                throw new System.Exception("Empty polygon");
            }
        }

        /// <summary>
        /// Finishes drawing the polygon mesh.
        /// </summary>
        private void FinishPolygonMesh()
        {
            // Set color for the finished polygon
            Color polygonColor = Color.green; // You can choose any color
            Mesh mesh = activePolygonMesh.meshFilter.mesh;
            mesh.colors = mesh.vertices.Select(_ => polygonColor).ToArray();
            activePolygonMesh.meshFilter.mesh = mesh;
            activePolygonMesh.UpdateMesh();
        }

        /// <summary>
        /// Whether the polygon being drawn is the positive polygon.
        /// </summary>
        /// <returns>Whether positivePolygon is null (meaning no polygon has been added to the room yet).</returns>
        public bool IsFirstPolygon() =>
            Room.Data.PositivePolygon == null;

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
            Room.Data = new();
            polygonDrawer.Reset();
            StartScanning();
        }

        /// <summary>
        /// Called on confirm button click.
        /// </summary>
        public void OnConfirmButtonClick()
        {
            switch (CurrentState)
            {
                case State.Drawing:
                    FinishPolygon();
                    SwitchToState(State.SettingHeight);
                    break;
                case State.SettingHeight:
                    AddPolygon(activePolygon);
                    FinishPolygonMesh();
                    SwitchToState(State.AskForNegPolygons);
                    break;
                case State.AskForNegPolygons:
                    StartScanning();
                    break;
            }
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