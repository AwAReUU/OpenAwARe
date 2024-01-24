// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.IO;
using System.Linq;

using AwARe.IngredientList.Objects;
using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.Objects;
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.UI;
using UnityEngine;
using UnityEngine.TestTools;
using AwARe.UI;
using System.Collections.Generic;
using System.Diagnostics;

using TMPro;
using System.Collections.Generic;

using AwARe.Data.Logic;

using Unity.IO.LowLevel.Unsafe;

using Room = AwARe.Data.Objects.Room;
using System.Collections;
using AwARe.Data.Objects;
using AYellowpaper;
using AwARe.UI.Objects;

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

        /// <summary>
        /// The session anchors used for saving/loading rooms.
        /// </summary>
        private List<Vector3> sessionAnchors = new();
        [SerializeField] private GameObject anchorVisual;

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

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss()
        {
            polygonManager.OnUIMiss();

            if (CurrentState == State.SaveAnchoring)
            {
                TryAddAnchor(pointer.PointedAt, anchorVisual);

                if (sessionAnchors.Count >= 2)
                {
                    OnSaveButtonClick();
                }
            }

            else if (CurrentState == State.LoadAnchoring)
            {
                TryAddAnchor(pointer.PointedAt, anchorVisual);

                if (sessionAnchors.Count >= 2)
                {
                    OnLoadButtonClick();
                }

            }
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
        /// <summary>
        /// get the room that is associated with the clicked button's name
        /// </summary>
        public Data.Logic.Room ChooseRoom(string name)
        {
            List<Data.Logic.Room> listofrooms = LoadRoomList();
            return listofrooms.Where(obj => obj.RoomName == name).SingleOrDefault();
        }

        [ExcludeFromCoverage]
        public void OnStartSavingButtonClick()
        {
            //Storage.Get().ActiveRoom = Room.Data;
            sessionAnchors.Clear();
            stateBefore = CurrentState;
            SwitchToState(State.SaveAnchoring);
        }

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
        public void UpdateRoomList(List<Data.Logic.Room>roomlist)
        {
            SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();
            RoomListSerialization serroomlist = new RoomListSerialization();
            foreach (Data.Logic.Room room in roomlist)
            {
                serroomlist.Rooms.Add(new RoomSerialization(room, sessionAnchors));
            }
            saveLoadManager.SaveRoomList("rooms",serroomlist);

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