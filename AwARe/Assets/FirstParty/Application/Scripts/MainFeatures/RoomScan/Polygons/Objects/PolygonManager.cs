// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;

using AwARe.Data.Logic;
using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Polygons.Logic;
using UnityEngine;
using Storage = AwARe.InterScenes.Objects.Storage;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using AwARe.IngredientList.Logic;
using System.IO;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

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
        [SerializeField] private PathVisualizer pathVisualizerVisualizer;
        [SerializeField] private PolygonSaveLoadManager SaveLoadManager;
        [SerializeField] private PolygonUI ui;
        [SerializeField] private GameObject canvas;
        [SerializeField] private Transform sceneCanvas;
        [SerializeField] private GameObject pathBtn;
        [SerializeField] private GameObject LoadingPopup;
        [SerializeField] private GameObject SavedPopup;




        private bool scanning = false;

        /// <value>A Room represented by the polygons.</value>
        public Room Room { get; private set; }

        /// <value>The polygon currently being drawn.</value>
        public Polygon CurrentPolygon { get; private set; }

        

        
        private void Awake()
        {

            if (canvas != null)
            {
                ui.transform.SetParent(sceneCanvas, false);
                Destroy(canvas);
            }

            // Temporary quick fix for loading and saving UI
            Button save1Btn = ui.transform.Find("SaveBtns").transform.GetChild(0).GetComponent<Button>();
            Button save2Btn = ui.transform.Find("SaveBtns").transform.GetChild(1).GetComponent<Button>();
            Button save3Btn = ui.transform.Find("SaveBtns").transform.GetChild(2).GetComponent<Button>();
            Button load1Btn = ui.transform.Find("LoadBtns").transform.GetChild(0).GetComponent<Button>();
            Button load2Btn = ui.transform.Find("LoadBtns").transform.GetChild(1).GetComponent<Button>();
            Button load3Btn = ui.transform.Find("LoadBtns").transform.GetChild(2).GetComponent<Button>();
            save1Btn.onClick.AddListener(() => { SaveRoom(1); ContinueToGeneration(); });
            save2Btn.onClick.AddListener(() => { SaveRoom(2); ContinueToGeneration(); });
            save3Btn.onClick.AddListener(() => { SaveRoom(3); ContinueToGeneration(); });
            load1Btn.onClick.AddListener(() => { LoadRoom(1); });
            load2Btn.onClick.AddListener(() => { LoadRoom(2); });
            load3Btn.onClick.AddListener(() => { LoadRoom(3); });

        }

        void Start()
        {
            CurrentPolygon = new Polygon();
            Room = new Room();


            // Use the code below to use the test room
            //Room = new TestRoom();
            //polygonDrawer.DrawRoomPolygons(Room);


            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == "RoomLoad")
            {
                Debug.Log("hey i am loading");
                SwitchToState(State.Loading);
            }
            else SwitchToState(State.Default);


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
            Debug.Log(SaveLoadManager);
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
            SwitchToState(State.LoadingOptions);
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
            //GenerateAndDrawPath();
        }

        public void OnPathButtonClick()
        {
            //activate the popup
            LoadingPopup.SetActive(true);
            StartCoroutine(MakePathAndRemovePopup());

        }

        
        public void OnSaveButtonClick()
        {
            SwitchToState(State.SavingOptions);
            
        }

        public void ContinueToGeneration()
        {
            Storage.Get().ActiveRoom = Room;
            SceneSwitcher.Get().LoadScene("Home");
        }
        

        public IEnumerator MakePathAndRemovePopup()
        {
            yield return null;
            GenerateAndDrawPath();
            LoadingPopup.SetActive(false);
        }

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

            PathVisualizer visualizer = (PathVisualizer)pathVisualizerVisualizer.GetComponent("PathVisualizer");
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
            SavedPopup.SetActive(true);

        }


        // Update SaveRoom and LoadRoom methods
        public void SaveRoom(int slotIndex)
        {
            // Save the current room directly using the save load manager

            // Convert Room to RoomSerialization
            RoomSerialization roomSerialization = new RoomSerialization(
                new PolygonSerialization(Room.PositivePolygon.Points),
                Room.NegativePolygons.Select(p => new PolygonSerialization(p.Points)).ToList()
            );

            // Save RoomSerialization
            SaveLoadManager.SaveDataToJson($"RoomSlot{slotIndex}", roomSerialization);

            Debug.Log($"Saved room in slot {slotIndex}");
        }
        public void LoadRoom(int slotIndex)
        {
            PolygonSaveLoadManager saveLoadManager = FindObjectOfType<PolygonSaveLoadManager>();
            Debug.Log($"Directory Path before load: {saveLoadManager.DirectoryPath}");

            // Check if the file exists before attempting to load
            string filePath = $"RoomSlot{slotIndex}";
            string fullPath = System.IO.Path.Combine(saveLoadManager.DirectoryPath, filePath);

            if (File.Exists(fullPath))
            {
                // Load RoomSerialization JSON using the save load manager
                RoomSerialization loadedRoomSerialization = saveLoadManager.LoadDataFromJson<RoomSerialization>($"RoomSlot{slotIndex}");
                Debug.Log($"Loaded JSON: {JsonUtility.ToJson(loadedRoomSerialization)}");

                if (loadedRoomSerialization != null)
                {
                    // Convert RoomSerialization to Room
                    Room = loadedRoomSerialization.ToRoom();

                    if (Room != null)
                    {
                        Debug.Log($"Loaded Room with {Room.NegativePolygons.Count} negative polygons.");

                        if (!Room.PositivePolygon.IsEmptyPolygon())
                        {
                            if (polygonDrawer != null)
                            {
                                //polygonDrawer.ClearScanningLines();
                                polygonMesh.SetPolygon(Room.PositivePolygon.GetPoints());
                                polygonDrawer.DrawRoomPolygons(Room);
                                GenerateAndDrawPath();

                                Debug.Log("Room loaded successfully.");
                            }
                            else
                            {
                                Debug.LogError("PolygonDrawer is null.");
                            }
                        }
                        else
                        {
                            Debug.LogError("Loaded room has an empty positive polygon.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Loaded room is null after conversion.");
                    }
                }
                else
                {
                    Debug.LogError("Loaded room serialization is null.");
                }
            }
            else
            {
                Debug.LogError($"Room not found in slot {slotIndex}");
            }
        }



        /// <summary>
        /// Called on changing the slider; sets the height of the polygon mesh.
        /// </summary>
        /// <param name="height">Height the slider is currently at.</param>
        public void OnHeightSliderChanged(float height) { this.polygonMesh.SetHeight(height); }

        public void OnEndButtonClick ()
        {
            //Storage.Get().ActiveRoom = Room;
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

            // if the new state is scanning, set scanning to true, otherwise to false
            scanning = toState == State.Scanning;
            polygonDrawer.ScanningPolygon = null;
        }

    

        /// <summary>
        /// Sets activity of components.
        /// </summary>
        /// <param name="state">Current/new state.</param>
        public void SetActive(State state)
        {
            // Set UI activity
            ui.SetActive(state);

            // Set direct component activity
            bool scan = false, mesh = false;
            switch (state)
            {
                case State.Scanning:
                    scan = true;
                    break;
                case State.SettingHeight:
                    mesh = true;
                    break;
                
                    
            }
            scanner.gameObject.SetActive(scan);
            polygonMesh.gameObject.SetActive(mesh);
        }
    }

    /// <summary>
    /// The different states within the polygon scanning process.
    /// </summary>
    public enum State
    {
        Default,
        Scanning,
        SettingHeight,
        Saving,
        SavingOptions,
        Loading,
        LoadingOptions
    }
}