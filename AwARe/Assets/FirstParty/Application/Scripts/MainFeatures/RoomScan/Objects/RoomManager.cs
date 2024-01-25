// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Logic;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.UI.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using Room = AwARe.Data.Objects.Room;

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
        [SerializeField] private RoomListOverviewScreen roomScreen;
        // [SerializeField] private VisualizePath pathVisualizer; //TODO: Get out of polygonScanning

        // The UI
        [SerializeField] private RoomUI ui;
        [SerializeField] private Transform canvas;
        [SerializeField] private Transform sceneCanvas;
        [SerializeField] private GameObject saveNameScreen;
        [SerializeField] public TMP_InputField inputName;

        // Templates
        [SerializeField] private GameObject roomBase;

        // The pointer
        [SerializeField] public Pointer pointer;

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

        #region Anchor Handling
        /// <summary>
        /// The session anchors used for saving/loading rooms.
        /// </summary>
        private List<Vector3> sessionAnchors = new();
        [SerializeField] private GameObject anchorVisual;

        private List<Texture2D> screenshots = new();

        /// <summary>
        /// Add an anchor to the sessionAnchors list, fails if list is full (2 anchors max.).
        /// </summary>
        /// <param name="anchorPoint"></param>
        /// <param name="anchorVisual"></param>
        public void TryAddAnchor(Vector3 anchorPoint, GameObject anchorVisual = null)
        {
            if (sessionAnchors.Count >= 2) return;

            //UnityEngine.Debug.Log("Anchor count: " + sessionAnchors.Count);
            sessionAnchors.Add(anchorPoint);
            if (anchorVisual != null)
                Instantiate(anchorVisual, anchorPoint, Quaternion.identity);
        }

        /// <summary>
        /// Remove the last anchor from the sessionAnchors.
        /// </summary>
        public void TryRemoveLastAnchor()
        {
            if (sessionAnchors.Count == 0) return;

            sessionAnchors.RemoveAt(sessionAnchors.Count - 1);

            // TODO remove visual
        }
        #endregion

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss()
        {
            polygonManager.OnUIMiss();

            OnSetPointButtonClick();
        }

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
        /// Called on set point button click; Places anchors.
        /// </summary>
        public void OnSetPointButtonClick()
        {
            if (ui.screenshotManager.screenshotDisplayed)
            {
                ui.screenshotManager.HideScreenshot();
                return;
            }

            if (CurrentState == State.SaveAnchoring)
            {
                TryAddAnchor(pointer.PointedAt, anchorVisual);

                Texture2D screenshot = ui.screenshotManager.TakeScreenshot();
                screenshots.Add(screenshot);

                ui.DisplayAnchorSavingImage(screenshot);

                if (sessionAnchors.Count == 2)
                {
                    OnSaveButtonClick();
                }
            }
            else if (CurrentState == State.LoadAnchoring)
            {
                if(sessionAnchors.Count >= 1)
                {
                    ui.DisplayAnchorLoadingImage(1);
                }
                else if (sessionAnchors.Count >= 2)
                {
                    LoadRoom();
                }
            }
        }

        public void OnYesButtonClick()
        {
            if(CurrentState == State.LoadAnchoring)
            {
                if (ui.screenshotManager.screenshotDisplayed)
                {
                    ui.screenshotManager.HideScreenshot();
                }
                else
                {

                }
            }
        }

        public void OnNoButtonClick()
        {
                if(sessionAnchors.Count >= 1)
                {
                    ui.DisplayAnchorLoadingImage(1);
                }
                else if (sessionAnchors.Count >= 2)
                {
                    LoadRoom();
                }
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
            SwitchToState(State.Saving);
            //roomScreen.DisplayRoomLists(roomScreen.roomList);
        }

        [ExcludeFromCoverage]
        public void OnStartSavingButtonClick()
        {
            //Storage.Get().ActiveRoom = Room.Data;
            sessionAnchors.Clear();
            stateBefore = CurrentState;
            SwitchToState(State.SaveAnchoring);
        }

        private string roomToLoad = "";
        public void StartLoadingRoom(string name)
        {
            roomToLoad = name;
            stateBefore = CurrentState;
            SwitchToState(State.LoadAnchoring);
        }

        private void LoadRoom()
        {
            //SwitchToState(State.Loading);

            Data.Logic.Room room;
            room = ChooseRoom(roomToLoad);
            MakeRoom(room);


            stateBefore = CurrentState;
            SwitchToState(State.Done);
            //TODO: Switch to AR Scene
        }

        /// <summary>
        /// get the room that is associated with the clicked button's name
        /// </summary>
        public Data.Logic.Room ChooseRoom(string name)
        {
            List<Data.Logic.Room> listofrooms = LoadRoomList();
            return listofrooms.Where(obj => obj.RoomName == name).SingleOrDefault();
        }

        /// <summary>
        /// Visualize the room in AR.
        /// </summary>
        /// <param name="room"></param>
        public void MakeRoom(Data.Logic.Room room)
        {
            ClearRoom();

            Room.Data = room;
            Room.positivePolygon.GetComponent<Mesher>().UpdateMesh();
            Room.positivePolygon.GetComponent<Liner>().UpdateLine();
            foreach (var polygon in Room.negativePolygons)
            {
                polygon.GetComponent<Mesher>().UpdateMesh();
                polygon.GetComponent<Liner>().UpdateLine();
            }
            Storage.Get().ActivePath = pathManager.GenerateAndDrawPath();
        }

        /// <summary>
        /// Clear room if new room is spawned so there is only one room at a time.
        /// </summary>
        private void ClearRoom()
        {
            // Destroy the existing positive polygon
            if (Room.positivePolygon != null)
                Destroy(Room.positivePolygon.gameObject);

            // Destroy the existing negative polygons
            if (Room.negativePolygons != null)
            {
                foreach (var polygon in Room.negativePolygons)
                {
                    if (polygon != null)
                        Destroy(polygon.gameObject);
                }
            }

        }

        /// <summary>
        /// Save newly created room in rooms file
        /// </summary>
        public void SaveClick()
        {
            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();
            UnityEngine.Debug.Log(Room.Data.RoomName);
            Storage.Get().ActiveRoom = Room.Data;
            Storage.Get().ActiveRoom.RoomName = inputName.text;

            stateBefore = CurrentState;

            // Load existing room list
            RoomListSerialization roomList = saveLoadManager.LoadRooms("rooms");

            if (roomList == null)
            {
                roomList = new RoomListSerialization();
            }

            // Add the current room to the list
            roomList.Rooms.Add(new RoomSerialization(Storage.Get().ActiveRoom, sessionAnchors));

            // Save the updated room list
            saveLoadManager.SaveRoomList("rooms", roomList);
            roomScreen.roomList = LoadRoomList();
            roomScreen.DisplayRoomLists(roomScreen.roomList);

            //SwitchToState(State.Default);

        }

        /// <summary>
        /// go from list of rooms to roomlist Serialization so that you can update the rooms file
        /// </summary>
        public void UpdateRoomList(List<Data.Logic.Room> roomlist)
        {
            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();
            RoomListSerialization serroomlist = new RoomListSerialization();
            foreach (Data.Logic.Room room in roomlist)
            {
                serroomlist.Rooms.Add(new RoomSerialization(room, sessionAnchors));
            }
            saveLoadManager.SaveRoomList("rooms", serroomlist);

        }

        /// <summary>
        /// load in a list of rooms from roomListSerialization rooms
        /// </summary>
        public List<Data.Logic.Room> LoadRoomList()
        {

            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();

            // Ensure that saveLoadManager is not null before proceeding
            if (saveLoadManager == null)
            {
                UnityEngine.Debug.LogError("SaveLoadManager is null.");
                return new List<Data.Logic.Room>();
            }

            RoomListSerialization roomListSerialization = saveLoadManager.LoadRooms("rooms");

            if (roomListSerialization == null)
            {
                //Debug.LogError("RoomListSerialization is null.");
                return new List<Data.Logic.Room>();
            }
            UnityEngine.Debug.Log(roomListSerialization.Rooms?.Select(roomSerialization => roomSerialization.ToRoom(sessionAnchors)).ToList() ?? new List<Data.Logic.Room>());

            // Convert RoomListSerialization to a list of Room objects
            return roomListSerialization.Rooms?.Select(roomSerialization => roomSerialization.ToRoom(sessionAnchors)).ToList() ?? new List<Data.Logic.Room>();
        }

        /// <summary>
        /// Called on load button button click; changes state so user sees load slots.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick()
        {
            stateBefore = CurrentState;
            SwitchToState(State.Loading);
        }

        [ExcludeFromCoverage]
        public void OnStartLoadingButtonClick()
        {
            sessionAnchors.Clear();
            stateBefore = CurrentState;
            SwitchToState(State.LoadAnchoring);
        }

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
        SaveAnchoring,
        LoadAnchoring
    }
}