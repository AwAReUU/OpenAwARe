// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Polygons.Objects;
using UnityEngine;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        // Objects to control
        [SerializeField] private PolygonManager polygonManager;
        [SerializeField] private VisualizePath pathVisualizer; //TODO: Get out of polygonScanning

        private void Awake()
        {
            var obj = new GameObject("Room");
            Room = obj.AddComponent<Room>();

            // Use the code below to use the test room
            //Room.Data = new TestRoom();
            //DrawAllPolygons(); ???
        }

        /// <summary>
        /// Gets or sets the current room.
        /// </summary>
        /// <value>
        /// The current room.
        /// </value>
        public Room Room { get; set; }

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss() =>
            polygonManager.OnUIMiss();

        /*
        // TODO: Move out of Polygon scanning classes.
        public void GenerateAndDrawPath()
        {
            PathGenerator startstate = new();

            bool useTestPol = false;

            List<Vector3> points = new()
            {
                //new Vector3(0.3290718f, 0, -1.92463f),
                //new Vector3(-0.5819738f, 0, -2.569284f),
                //new Vector3(3.357841f, 0, -3.58952f),
                //new Vector3(3.824386f, 0, -2.0016f),

                new Vector3(-3.330043f, 0, -3.042626f),
                new Vector3(-2.702615f, 0, -5.299197f),
                new Vector3(-1.407629f, 0, -4.649026f),
                new Vector3(-0.4994112f, 0, -2.780823f),
                new Vector3(-2.009388f, 0, -0.5163946f),

            };

            Data.Logic.Polygon testPolygon = new Data.Logic.Polygon(points);

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
        */
    }
}