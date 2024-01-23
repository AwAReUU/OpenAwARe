// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.IO;
using System.Linq;
using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.UI;
using UnityEngine;
using UnityEngine.TestTools;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        // Objects to control
        [SerializeField] private PolygonManager polygonManager;
        [SerializeField] private PathManager pathManager;
        // [SerializeField] private VisualizePath pathVisualizer; //TODO: Get out of polygonScanning

        // The UI
        [SerializeField] private RoomUI ui;
        [SerializeField] private Transform canvas;
        [SerializeField] private Transform sceneCanvas;

        // Templates
        [SerializeField] private GameObject roomBase;

        // Tracking
        private State stateBefore;

        /// <summary>
        /// Gets the current state of the room scanner.
        /// </summary>
        /// <value>
        /// The current state of the room scanner.
        /// </value>
        public State CurrentState { get; private set; } = State.Default;

        [ExcludeFromCoverage]
        private void Awake()
        {
            // Move all content prefab canvas to scene canvas.
            Mover.MoveAllChildren(canvas, sceneCanvas, true);

            // Instantiate a room to construct.
            Room = Instantiate(roomBase, transform).GetComponent<Room>();

            SwitchToState(State.Default);
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

        /// <summary>
        /// Called on create button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnCreateButtonClick()
        {
            SwitchToState(State.Scanning);
            polygonManager.OnCreateButtonClick();
        }

        /// <summary>
        /// Called on reset button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnResetButtonClick() =>
            polygonManager.OnResetButtonClick();


        /// <summary>
        /// Called on apply button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnApplyButtonClick() =>
            polygonManager.OnApplyButtonClick();

        /// <summary>
        /// Called on confirm button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnConfirmButtonClick()
        {
            polygonManager.OnConfirmButtonClick();
            SwitchToState(State.Done);
        }

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnHeightSliderChanged(float value) =>
            polygonManager.OnHeightSliderChanged(value);

        /// <summary>
        /// Called on save button click; Stores the current room and switches to the home screen.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnSaveButtonClick()
        {
            Storage.Get().ActiveRoom = Room.Data;
            stateBefore = CurrentState;
            SwitchToState(State.Saving);
        }

        /// <summary>
        /// Called on save slot click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnSaveSlotClick(int slotIdx) =>
            SaveRoom(slotIdx);

        /// <summary>
        /// Called on load button button click; changes state so user sees load slots.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick()
        {
            stateBefore = CurrentState; 
            SwitchToState(State.Loading);
        }

        /// <summary>
        /// Called on load slot click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadSlotClick(int slotIdx) =>
            LoadRoom(slotIdx);

        /// <summary>
        /// Called on continue button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnContinueClick() =>
            SwitchToState(stateBefore);

        /// <summary>
        /// Called on path button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnPathButtonClick() =>
            pathManager.OnPathButtonClick();

        /// <summary>
        /// Sets all Objects activities to match new state.
        /// </summary>
        /// <param name="state">The new state switched to.</param>
        private void SwitchToState(State state)
        {
            CurrentState = state;
            SetActive();
        }

        /// <summary>
        /// Sets activity of components.
        /// </summary>
        /// <param name="state">Current/new state.</param>
        [ExcludeFromCoverage]
        public void SetActive()
        {
            if (CurrentState == State.Scanning && !(pathManager.IsActive || polygonManager.IsActive))
                CurrentState = State.Done;
            
            // Set UI activity
            ui.SetActive(this.CurrentState, polygonManager.CurrentState, pathManager.CurrentState);
        }

        /// <summary>
        /// Saves the current room's configuration to a specified save slot using the save load manager.
        /// </summary>
        /// <param name="slotIndex">The index of the save slot to store the room configuration.</param>
        public void SaveRoom(int slotIndex)
        {
            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();

            // Convert Room to RoomSerialization
            RoomSerialization roomSerialization = new(Room.Data);

            // Save RoomSerialization
            saveLoadManager.SaveDataToJson($"RoomSlot{slotIndex}", roomSerialization);
        }

        /// <summary>
        /// Loads a previously saved room configuration from a specified save slot using the save load manager.
        /// </summary>
        /// <param name="slotIndex">The index of the save slot from which to load the room configuration.</param>
        public void LoadRoom(int slotIndex)
        {
            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();

            // Check if the file exists before attempting to load
            string filePath = $"RoomSlot{slotIndex}";
            string fullPath = System.IO.Path.Combine(saveLoadManager.DirectoryPath, filePath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Room not found in slot {slotIndex}");
                return;
            }

            // Load RoomSerialization JSON using the save load manager
            RoomSerialization loadedRoomSerialization = saveLoadManager.LoadDataFromJson<RoomSerialization>($"RoomSlot{slotIndex}");

            if (loadedRoomSerialization == null)
            {
                Debug.LogError("Loaded room serialization is null.");
                return;
            }

            // Convert RoomSerialization to Room
            if(Room != null) Destroy(Room.gameObject);
            Room = Instantiate(roomBase, transform).GetComponent<Room>();
            Room.Data = loadedRoomSerialization.ToRoom();

            if (Room.Data == null)
            {
                Debug.LogError("Loaded room is null after conversion.");
                return;
            }
            if (Room.positivePolygon == null || Room.positivePolygon.Data.points.Count == 0)
            {
                Debug.LogError("Loaded room does not have a positive polygon.");
                return;
            }

            Room.positivePolygon.GetComponent<Mesher>().UpdateMesh();
            Room.positivePolygon.GetComponent<Liner>().UpdateLine();
            foreach (var polygon in Room.negativePolygons) {
                polygon.GetComponent<Mesher>().UpdateMesh(); 
                polygon.GetComponent<Liner>().UpdateLine();
            }
            pathManager.GenerateAndDrawPath();
        }
    }

    /// <summary>
    /// The different states within the Room scanning process.
    /// </summary>
    public enum State
    {
        Default,
        Scanning,
        Done,
        Saving,
        Loading,
    }
}