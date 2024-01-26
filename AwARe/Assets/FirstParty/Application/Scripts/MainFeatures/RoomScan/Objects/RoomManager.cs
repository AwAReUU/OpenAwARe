// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AwARe.Data.Logic;
using AwARe.Data.Objects;
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

        private SaveLoadManager saveLoadManager;

        /// <summary>
        /// The state the scene should start in.
        /// </summary>
        public State startState = State.Default;

        /// <summary>
        /// The previous state; used for tracking.
        /// </summary>
        private State stateBefore;

        /// <summary>
        /// Gets the current state of the room scanner.
        /// </summary>
        /// <value>
        /// The current state of the room scanner.
        /// </value>
        public State CurrentState { get; private set; }

        [ExcludeFromCoverage]
        private void Awake()
        {
            // Move all content prefab canvas to scene canvas.
            Mover.MoveAllChildren(canvas, sceneCanvas, true);

            // Instantiate a room to construct.
            Room = Instantiate(roomBase, transform).GetComponent<Room>();

            saveLoadManager = new();
            SwitchToState(startState);
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

        /// <summary>
        /// The screenshots used for saving/loading rooms.
        /// </summary>
        private List<Texture2D> screenshots = new();

        /// <summary>
        /// Add an anchor to the sessionAnchors list, fails if list is full (2 anchors max.).
        /// </summary>
        /// <param name="anchorPoint">The location of the anchor.</param>
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
            //polygonManager.OnUIMiss();
        }

        /// <summary>
        /// Called on create button click.
        /// </summary>
        public void OnCreateButtonClick()
        {
            if(CurrentState == State.Default || CurrentState == State.AskForSave)
            {
                SwitchToState(State.Scanning);
                polygonManager.OnCreateButtonClick();
            }
            else if (CurrentState == State.Loading)
            {
                SwitchToState(State.Scanning);
                polygonManager.OnCreateButtonClick();
            }
        }

        /// <summary>
        /// Called on reset button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnResetButtonClick() =>
            polygonManager.OnResetButtonClick();

        /// <summary>
        /// Called on confirm button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnConfirmButtonClick()
        {
            if(polygonManager.CurrentState == Polygons.State.Drawing)
                polygonManager.OnApplyButtonClick();
            if (CurrentState == State.Scanning)
            {
                polygonManager.OnConfirmButtonClick();
                pathManager.OnPathButtonClick();
                SwitchToState(State.AskForSave);
            }
            else if(CurrentState == State.SaveAnchoringCheck)
            {
                ui.screenshotManager.HideScreenshot();
                if(sessionAnchors.Count >= 2)
                {
                    for(int i = 0; i < screenshots.Count; i++)
                    {
                        ui.screenshotManager.SaveScreenshot(screenshots[i], Room.Data, i);
                    }
                    SwitchToState(State.Saving);
                }
                else
                {
                    SwitchToState(State.SaveAnchoring);
                }
            }
        }

        /// <summary>
        /// Called on set point button click; Places anchors.
        /// </summary>
        public void OnSetPointButtonClick()
        {
            if (CurrentState == State.Scanning)
            {
                polygonManager.TryAddPoint();
            }
            else if (CurrentState == State.SaveAnchoring)
            {
                TryAddAnchor(pointer.PointedAt, anchorVisual);

                Texture2D screenshot = ui.screenshotManager.TakeScreenshot();
                screenshots.Add(screenshot);

                ui.DisplayAnchorSavingImage(screenshot);

                SwitchToState(State.SaveAnchoringCheck);
            }
            else if (CurrentState == State.LoadAnchoring)
            {
                TryAddAnchor(pointer.PointedAt, anchorVisual);
                screenshots.Add(ui.screenshotManager.TakeScreenshot());

                if(sessionAnchors.Count < 2)
                    ui.DisplayAnchorLoadingImage(sessionAnchors.Count);
                else
                {
                    LoadRoom();
                }
            }
        }

        public void OnNoButtonClick()
        {
            if (CurrentState == State.AskForSave)
            {
                LoadRoom();
            }
            else if(CurrentState == State.SaveAnchoringCheck)
            {
                sessionAnchors.RemoveAt(sessionAnchors.Count - 1);
                screenshots.RemoveAt(screenshots.Count - 1);
                SwitchToState(State.SaveAnchoring);
            }
        }

        [ExcludeFromCoverage]
        public void OnSaveButtonClick()
        {
            //Storage.Get().ActiveRoom = Room.Data;
            sessionAnchors.Clear();
            stateBefore = CurrentState;
            SwitchToState(State.SaveAnchoring);
        }

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnHeightSliderChanged(float value) =>
            polygonManager.OnHeightSliderChanged(value);

        private string roomToLoad = "";
        public void StartLoadingRoom(string name)
        {
            roomToLoad = name;
            stateBefore = CurrentState;
            SwitchToState(State.LoadAnchoring);
        }

        public void SetActiveRoom()
        {
            Data.Logic.Room room;
            room = ChooseRoom(roomToLoad);

            Storage.Get().ActiveRoom = room;
            Storage.Get().ActivePath = pathManager.GenerateAndDrawPath();
        }

        public void LoadRoom()
        {
            SetActiveRoom();
            
            SceneSwitcher.Get().LoadScene("AR");
        }

        /// <summary>
        /// get the room that is associated with the clicked button's name.
        /// </summary>
        /// <param name="name">The clicked button's name.</param>
        public Data.Logic.Room ChooseRoom(string name)
        {
            List<Data.Logic.Room> listofrooms = LoadRoomList();
            return listofrooms.Where(obj => obj.RoomName == name).SingleOrDefault();
        }

        /// <summary>
        /// Visualize the room in AR.
        /// </summary>
        /// <param name="room">The room to visualize.</param>
        public void VisualizeRoom(Data.Logic.Room room)
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
        /// Save newly created room in rooms file.
        /// </summary>
        public void SaveClick()
        {
            Debug.Log("inputNametext: " + inputName.text);
            Debug.Log("nullcheck: " + Room.Data.RoomName == null);
            //Room.Data.RoomName = inputName.text;
            //Debug.Log("managerRoomname: " + Room.Data.RoomName);
            Room.roomName = inputName.text;
            Debug.Log("managerRoomname: " + Room.roomName);
            Storage.Get().ActiveRoom = Room.Data;
            Storage.Get().ActivePath = pathManager.GenerateAndDrawPath();

            stateBefore = CurrentState;

            // Load existing room list
            RoomListSerialization roomList = saveLoadManager.LoadRooms("rooms");

            roomList ??= new RoomListSerialization();

            // Add the current room to the list
            roomList.Rooms.Add(new RoomSerialization(Storage.Get().ActiveRoom, sessionAnchors));

            // Save the updated room list
            saveLoadManager.SaveRoomList("rooms", roomList);
            roomScreen.roomList = LoadRoomList();
            roomScreen.DisplayRoomLists(roomScreen.roomList);

            //SwitchToState(State.Default);

            //LoadRoom();
        }

        /// <summary>
        /// Remove the screenshots in the given room.
        /// </summary>
        /// <param name="room">The room that is being deleted.</param>
        public void DeleteRoom(Data.Logic.Room room)
        {
            ui.screenshotManager.DeleteScreenshot(room, 0);
            ui.screenshotManager.DeleteScreenshot(room, 1);
        }

        /// <summary>
        /// Go from list of rooms to roomlist Serialization so that you can update the rooms file.
        /// </summary>
        public void UpdateRoomList(List<Data.Logic.Room> roomlist)
        {
            RoomListSerialization serroomlist = new RoomListSerialization();
            foreach (Data.Logic.Room room in roomlist)
            {
                serroomlist.Rooms.Add(new RoomSerialization(room, sessionAnchors));
            }
            saveLoadManager.SaveRoomList("rooms", serroomlist);

        }

        /// <summary>
        /// Load in a list of rooms from roomListSerialization rooms.
        /// </summary>
        /// <returns>The list of rooms.</returns>
        public List<Data.Logic.Room> LoadRoomList()
        {

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

        /// <summary>
        /// Called on start loading button click.
        /// </summary>
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
                CurrentState = State.AskForSave;

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
        AskForSave,
        Saving,
        Loading,
        SaveAnchoring,
        SaveAnchoringCheck,
        LoadAnchoring
    }
}