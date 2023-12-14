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
using UnityEngine.UI;
using AwARe.IngredientList.Logic;
using System.IO;
using System.Linq;

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
        [SerializeField] private VisualizePath pathVisualizer;

        [SerializeField] private GameObject createBtn;
        [SerializeField] private GameObject saveBtns;
        [SerializeField] private GameObject saveBtn;
        [SerializeField] private GameObject loadBtn;
        [SerializeField] private GameObject resetBtn;
        [SerializeField] private GameObject applyBtn;
        [SerializeField] private GameObject confirmBtn;
        [SerializeField] private GameObject endBtn;
        [SerializeField] private GameObject slider;
        [SerializeField] private GameObject pointerObj;

        private bool scanning = false;
        [SerializeField] private GameObject loadBtns;
// TODO TEMP        [SerializeField] private GameObject pathVisualiser;

        /// <summary>
        /// All UI components of the polygon scan.
        /// </summary>
        List<GameObject> UIObjects;

        /// <value>A Room represented by the polygons.</value>
        public Room Room { get; private set; }

        /// <value>The polygon currently being drawn.</value>
        public Polygon CurrentPolygon { get; private set; }
        private PolygonDrawer polydrawer;

        
        void Start()
        {
            CurrentPolygon = new Polygon();

            Room = new Room();
          

            // Use the code below to use the test room
            //Room = new TestRoom();
            //polygonDrawer.DrawRoomPolygons(Room);

            UIObjects = new()
            {
                createBtn,loadBtn, saveBtns, loadBtns, resetBtn, confirmBtn, slider, applyBtn, endBtn,
                pointerObj, scanner.gameObject, polygonMesh.gameObject   // TODO TEMP   , pathVisualiser
            };

            SwitchToState(State.Default);

            Button sav1Btn = saveBtns.transform.GetChild(0).GetComponent<Button>();
            Button sav2Btn = saveBtns.transform.GetChild(1).GetComponent<Button>();
            Button sav3Btn = saveBtns.transform.GetChild(2).GetComponent<Button>();

            Button load1Btn = loadBtns.transform.GetChild(0).GetComponent<Button>();
            Button load2Btn = loadBtns.transform.GetChild(1).GetComponent<Button>();
            Button load3Btn = loadBtns.transform.GetChild(2).GetComponent<Button>();
            

            sav1Btn.onClick.AddListener(() => SavePolygon(1));
            sav2Btn.onClick.AddListener(() => SavePolygon(2));
            sav3Btn.onClick.AddListener(() => SavePolygon(3));

            load1Btn.onClick.AddListener(() => LoadPolygon(1));
            load2Btn.onClick.AddListener(() => LoadPolygon(2));
            load3Btn.onClick.AddListener(() => LoadPolygon(3));
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

        public void OnLoadButtonClick()
        {
            loadBtns.SetActive(true);
        }

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



            GenerateAndDrawPath();
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
            // Set color for the finished polygon
            Color polygonColor = Color.green; // You can choose any color
            polygonMesh.SetPolygonColor(polygonColor);

            // Assuming you have a method to apply the color to the mesh
            polygonMesh.ApplyColorToMesh();

        }

        public void SavePolygon(int slotIndex)
        {
            PolygonSaveLoadManager saveLoadManager = FindObjectOfType<PolygonSaveLoadManager>();
            Debug.Log($"Directory Path before save: {saveLoadManager.DirectoryPath}");

            // Save the current polygon directly using the save load manager
            PolygonSerialization polygonSerialization = new PolygonSerialization(CurrentPolygon.Points);
            saveLoadManager.SaveDataToJson($"PolygonSlot{slotIndex}", polygonSerialization);
            Debug.Log($"Saved polygon in slot {slotIndex}");
        }

        public void LoadPolygon(int slotIndex)
        {
            PolygonSaveLoadManager saveLoadManager = FindObjectOfType<PolygonSaveLoadManager>();
            Debug.Log($"Directory Path before load: {saveLoadManager.DirectoryPath}");

            // Check if the file exists before attempting to load
            string filePath = $"PolygonSlot{slotIndex}";
            string fullPath = System.IO.Path.Combine(saveLoadManager.DirectoryPath, filePath);

            if (File.Exists(fullPath))
            {
                // Load the polygon JSON using the save load manager
                PolygonSerialization loadedPolygonSerialization = saveLoadManager.LoadDataFromJson<PolygonSerialization>($"PolygonSlot{slotIndex}");
                Debug.Log($"Loaded JSON: {JsonUtility.ToJson(loadedPolygonSerialization.Points)}");

                if (loadedPolygonSerialization != null)
                {
                    // Convert PolygonSerialization to Polygon
                    Polygon loadedPolygon = loadedPolygonSerialization.ToPolygon();

                    if (loadedPolygon != null && loadedPolygon.AmountOfPoints() > 0)
                    {
                        // Draw the loaded polygon
                        polydrawer.DrawPolygon(loadedPolygon);
                    }
                    else
                    {
                        Debug.LogError("Loaded polygon is invalid.");
                    }
                }
                else
                {
                    Debug.LogError($"Loaded polygon JSON is null or empty.");
                }
            }
            else
            {
                Debug.LogError($"Polygon not found in slot {slotIndex}");
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

        public void OnEndButtonClick ()
        {
            //Storage.Get().ActiveRoom = Room;
            SceneSwitcher.Get().LoadScene("Home");

        }
        public void OnSaveButtonClick()
        {
            saveBtns.SetActive(true);
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
                    objects.Add(loadBtn);
                    break;
                case State.Scanning:
                    objects.Add(loadBtn);
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
                    objects.Add(saveBtn);
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