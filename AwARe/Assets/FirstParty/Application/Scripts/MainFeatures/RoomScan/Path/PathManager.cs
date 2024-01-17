// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;

using AwARe.Objects;
using UnityEngine;

using Polygon = AwARe.Data.Objects.Polygon;
using Room = AwARe.Data.Objects.Room;
using Storage = AwARe.InterScenes.Objects.Storage;

namespace AwARe.RoomScan.Path.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Path Generation.
    /// </summary>
    public class PathManager : MonoBehaviour
    {
        // The upper management
        [SerializeField] private RoomScan.Objects.RoomManager manager;

        // Objects to control
        [SerializeField] private PathVisualizer pathVisualizerVisualizer;
        //[SerializeField] private VisualizePath pathVisualizer; //TODO: Get out of polygonScanning

        // Test Objects for development
        public Polygon testPolygon;

        public PathData path;

        /// <summary>
        /// Gets the current state of the path generator.
        /// </summary>
        /// <value>
        /// The current state of the path generator.
        /// </value>
        public State CurrentState { get; private set; }

        public bool IsActive =>
            CurrentState is State.Default or State.Done;

        /// <summary>
        /// Gets the currently active room.
        /// </summary>
        /// <value>
        /// A Room represented by the polygons.
        /// </value>
        public Room Room { get => manager.Room; private set => manager.Room = value; }

        public void OnPathButtonClick()
        {
            //activate the popup
            SwitchToState(State.Generating);
            StartCoroutine(MakePathAndRemovePopup());
        }

        public IEnumerator MakePathAndRemovePopup()
        {
            yield return null;
            GenerateAndDrawPath();
            SwitchToState(State.Default);
        }

        public void GenerateAndDrawPath()
        {
            PathGenerator startstate = new();

            bool useTestPol = testPolygon != null;
            Data.Logic.Room roomData = Room.Data;
            Data.Logic.Polygon positivePolygon = roomData.PositivePolygon;

            if (useTestPol)
            {
                List<Vector3> points = new()
                {
                    //new(0.3290718f, 0, -1.92463f),
                    //new(-0.5819738f, 0, -2.569284f),
                    //new(3.357841f, 0, -3.58952f),
                    //new(3.824386f, 0, -2.0016f),

                    new(-3.330043f, 0, -3.042626f),
                    new(-2.702615f, 0, -5.299197f),
                    new(-1.407629f, 0, -4.649026f),
                    new(-0.4994112f, 0, -2.780823f),
                    new(-2.009388f, 0, -0.5163946f),

                };

                testPolygon.Data = new(points, 1f);
                positivePolygon = testPolygon.Data;
            }
            
            path = startstate.GeneratePath(positivePolygon, roomData.NegativePolygons);

            if (useTestPol)
                testPolygon.GetComponent<Liner>().UpdateLine();

            PathVisualizer visualizer = pathVisualizerVisualizer.GetComponent<PathVisualizer>();
            visualizer.SetPath(path);
            visualizer.Visualize();
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