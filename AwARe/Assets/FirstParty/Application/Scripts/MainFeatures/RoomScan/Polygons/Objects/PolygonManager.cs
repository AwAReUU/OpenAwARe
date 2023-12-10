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
using UnityEngine.XR.ARFoundation;


namespace AwARe.RoomScan.Polygons.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the polygon scanning.
    /// </summary>
    public class PolygonManager : MonoBehaviour
    {
        [SerializeField] private ARAnchorManager anchorManager;
        [SerializeField] private PolygonDrawer polygonDrawer;
        [SerializeField] private PolygonMesh polygonMesh;
        [SerializeField] private PolygonScan scanner;

        [SerializeField] private GameObject createBtn;
        [SerializeField] private GameObject loadBtn;
        [SerializeField] private GameObject resetBtn;
        [SerializeField] private GameObject applyBtn;
        [SerializeField] private GameObject confirmBtn;
        [SerializeField] private GameObject endBtn;
        [SerializeField] private GameObject slider;
        [SerializeField] private GameObject pointerObj;
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
                createBtn,loadBtn, resetBtn, confirmBtn, slider, applyBtn, endBtn,
                pointerObj, scanner.gameObject, polygonMesh.gameObject   // TODO TEMP   , pathVisualiser
            };

            SwitchToState(State.Default);
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

        public void OnLoadButtonClick() =>
              LoadAnchor(Storage.Get().SavedAnchorId);

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

            // Save the anchor for the current polygon
            SaveAnchor(CurrentPolygon);

            GenerateAndDrawPath();
        }

        public void GenerateAndDrawPath()
        {
            StartState startstate = new();
            PathData path = startstate.GetStartState(Room.PositivePolygon, Room.NegativePolygons);

// TODO TEMP            VisualizePath visualizer = (VisualizePath)pathVisualiser.GetComponent("VisualizePath");
// TODO TEMP            visualizer.SetPath(path);
// TODO TEMP            visualizer.Visualize();
        }

        /// <summary>
        /// Called on confirm button click; sets the height of the polygon.
        /// </summary>
        public void OnConfirmButtonClick()
        {
            // TODO: set room height
            SwitchToState(State.Saving);
        }

        private void SaveAnchor(Polygon polygon)
        {
            if (polygon == null)
            {
                // Log an error or handle the case where the polygon is null
                Debug.LogError("Cannot save anchor for a null polygon.");
                return;
            }

            Vector3 anchorPosition = polygon.GetFirstPoint();

            Quaternion anchorRotation = Quaternion.identity;

            // Create Pose using position and rotation
            Pose anchorPose = new Pose(anchorPosition, anchorRotation);

            // Ensure anchorManager is not null
            if (anchorManager == null)
            {
                Debug.LogError("ARAnchorManager is null. Make sure it is assigned in the inspector.");
                return;
            }

            // Create and attach ARAnchor to the ARAnchorManager
            ARAnchor anchor = anchorManager.AddAnchor(anchorPose);

            // Check if the anchor is null (AddAnchor can fail)
            if (anchor == null)
            {
                // Log an error or handle the case where the anchor creation fails
                Debug.LogError("Failed to create anchor.");
                return;
            }

            // Ensure Storage.Get() and SavedPolygons are not null
            var storage = Storage.Get();
            if (storage == null || storage.SavedPolygons == null)
            {
                Debug.LogError("Storage or SavedPolygons is null. Make sure it is properly initialized.");
                return;
            }

            // Get the unique identifier of the anchor
            string anchorId = anchor.trackableId.ToString();

            // Save the anchor's unique identifier for later retrieval
            storage.SavedAnchorId = anchorId;

            // Serialize the polygon to JSON
            string polygonJson = polygon.ToJson();

            // Save the anchor ID and corresponding polygon JSON to the dictionary
            storage.SavedPolygons[anchorId] = polygonJson;

        }

        // Function to load the anchor and reposition the polygon
        private void LoadAnchor(string anchorId)
        {

            if (!string.IsNullOrEmpty(anchorId))
            {
                ARAnchor anchor = null;

                // Iterate over the trackables to find the anchor by trackableId
                foreach (var trackable in anchorManager.trackables)
                {
                    if (trackable.trackableId.ToString() == anchorId)
                    {
                        anchor = (ARAnchor)trackable;
                        break;
                    }
                }

                if (anchor != null)
                {
                    // Move the polygon to the saved anchor position
                    Vector3 savedPosition = anchor.transform.position;
                    polygonDrawer.MovePolygon(savedPosition);

                    // Retrieve the saved polygon JSON from the dictionary
                    if (Storage.Get().SavedPolygons.TryGetValue(anchorId, out string polygonJson))
                    {
                        // Deserialize the polygon and update the current polygon
                        Polygon savedPolygon = Polygon.FromJson(polygonJson);
                        CurrentPolygon = savedPolygon;
                    }
                    else
                    {
                        // Handle case where saved polygon data is not found
                    }
                }
                else
                {
                    // Handle case where anchor could not be resolved (perhaps it was removed or not created)
                }
            }
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
                    objects.Add(loadBtn);
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