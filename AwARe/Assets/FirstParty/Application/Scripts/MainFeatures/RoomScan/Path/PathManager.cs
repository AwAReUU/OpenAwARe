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
        [SerializeField] private PathVisualizer pathVisualizer;

        // Test Object for development
        public Polygon testPolygon;

        /// <summary>
        /// The path.
        /// </summary>
        public PathData path;

        /// <summary>
        /// Gets the current state of the path generator.
        /// </summary>
        /// <value>
        /// The current state of the path generator.
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
        /// Activates the popup and starts the path generation process.
        /// </summary>
        public void StartPathGen()
        {
            //activate the popup
            SwitchToState(State.Generating);
            StartCoroutine(MakePathAndRemovePopup());
        }

        /// <summary>
        /// Creates the path and removes the popup.
        /// </summary>
        private IEnumerator MakePathAndRemovePopup()
        {
            yield return null;
            Storage.Get().ActivePath = GenerateAndDrawPath();
            SwitchToState(State.Default);
        }

        /// <summary>
        /// Creates the path.
        /// </summary>
        /// <returns>The path.</returns>
        public PathData GenerateAndDrawPath()
        {
            PathGenerator startstate = new();

            bool useTestPol = testPolygon != null;
            Data.Logic.Room roomData = Room.Data;
            Data.Logic.Polygon positivePolygon = roomData.PositivePolygon;

            if (useTestPol)
            {
                List<Vector3> points = new()
                {
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

            PathVisualizer visualizer = pathVisualizer.GetComponent<PathVisualizer>();
            visualizer.SetPath(path);
            visualizer.Visualize();

            return path;
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