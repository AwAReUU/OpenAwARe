// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.Data.Logic;
using AwARe.InterScenes.Objects;
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.UI.Objects;
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
        [SerializeField] private RoomListManager roomListManager;

        // The UI
        [SerializeField] private RoomUI ui;
        [SerializeField] private Transform canvas;
        [SerializeField] private Transform sceneCanvas;
        [SerializeField] private RoomOverviewScreen roomOverviewScreen;

        // Templates
        [SerializeField] private GameObject roomBase;

        // The pointer
        [SerializeField] public Pointer pointer;

        /// <summary>
        /// The state the scene should start in.
        /// </summary>
        public State startState = State.Default;

        public RoomListSerialization RoomListSerialization => roomListManager.GetSerRoomList();

        private readonly int anchorCount = 2;

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
            ui.gameObject.SetActive(true);

            // Move all content prefab canvas to scene canvas.
            Mover.MoveAllChildren(canvas, sceneCanvas, true);

            // Instantiate a room to construct.
            Room = Instantiate(roomBase, transform).GetComponent<Room>();

            SwitchToState(startState);
        }

        /// <summary>
        /// Room used for saving.
        /// </summary>
        /// <value>
        /// The current room.
        /// </value>
        public Room Room { get; set; }

        /// <summary>
        /// Room used for loading.
        /// </summary>
        public RoomSerialization SerRoom { get; set; }

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
            if (sessionAnchors.Count >= anchorCount) return;

            sessionAnchors.Add(anchorPoint);
            if (anchorVisual != null)
            {
                GameObject anchorVisualObject;
                anchorVisualObject = Instantiate(anchorVisual, anchorPoint, Quaternion.identity) as GameObject;
                anchorVisualObject.transform.parent = transform;
                anchorVisualObject.name = "Anchor_" + sessionAnchors.Count.ToString();
            }
        }

        /// <summary>
        /// Remove the last anchor from the sessionAnchors.
        /// </summary>
        public void TryRemoveLastAnchor()
        {
            if (sessionAnchors.Count == 0) return;

            GameObject anchorVisualObject = GameObject.Find("Anchor_" + sessionAnchors.Count.ToString());
            if (anchorVisualObject != null)
            {
                Destroy(anchorVisualObject);
            }

            sessionAnchors.RemoveAt(sessionAnchors.Count - 1);
        }
        #endregion

        /// <summary>
        /// Called on create button click.
        /// </summary>
        public void OnCreateButtonClick()
        {
            if (CurrentState == State.Default || CurrentState == State.AskForSave)
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
            if (polygonManager.CurrentState == Polygons.State.Drawing)
                polygonManager.OnApplyButtonClick();
            else if (CurrentState == State.Scanning)
            {
                polygonManager.OnConfirmButtonClick();
                SwitchToState(State.AskForSave);
            }
            else if (CurrentState == State.SaveAnchoringCheck)
            {
                ui.screenshotManager.HideScreenshot();
                if (sessionAnchors.Count >= anchorCount)
                {
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
        public void OnSelectButtonClick()
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

                if (sessionAnchors.Count < anchorCount)
                    ui.DisplayAnchorLoadingImage(sessionAnchors.Count);
                else
                {
                    Data.Logic.Room room = roomListManager.LoadRoom(SerRoom, sessionAnchors);
                    GoToRoom(room);
                }
            }
        }

        public void OnNoButtonClick()
        {
            if (CurrentState == State.AskForSave)
            {
                GoToRoom(Room.Data);
            }
            else if (CurrentState == State.SaveAnchoringCheck)
            {
                //sessionAnchors.RemoveAt(sessionAnchors.Count - 1);
                TryRemoveLastAnchor();
                screenshots.RemoveAt(screenshots.Count - 1);
                SwitchToState(State.SaveAnchoring);
            }
        }

        [ExcludeFromCoverage]
        public void OnSaveButtonClick()
        {
            sessionAnchors.Clear();
            SwitchToState(State.SaveAnchoring);
        }

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnHeightSliderChanged(float value) =>
            polygonManager.OnHeightSliderChanged(value);

        public void StartLoadingRoom(int roomIndex)
        {
            SerRoom = roomListManager.GetSerRoomList().Rooms[roomIndex];
            sessionAnchors.Clear();
            screenshots.Clear();
            SwitchToState(State.LoadAnchoring);
        }

        public void GoToRoom(Data.Logic.Room room)
        {
            Storage.Get().ActiveRoom = room;
            Storage.Get().ActivePath = pathManager.GenerateAndDrawPath();
            SceneSwitcher.Get().LoadScene("AR");
        }

        /// <summary>
        /// Save newly created room in rooms file.
        /// </summary>
        public void SaveRoom()
        {
            Room.roomName = roomOverviewScreen.nameInput.text;
            roomListManager.SaveRoom(Room.Data, sessionAnchors, screenshots);

            roomOverviewScreen.DisplayList();
            
            GoToRoom(Room.Data);
        }

        /// <summary>
        /// Deletes the data and screenshots of the given room.
        /// </summary>
        /// <param name="room">The room that is being deleted.</param>
        public void DeleteRoom(int roomIndex)
        {
            roomListManager.DeleteRoom(roomIndex, anchorCount);
            roomOverviewScreen.DisplayList();
        }

        /*
        /// <summary>
        /// Go from list of rooms to roomlist Serialization so that you can update the rooms file.
        /// </summary>
        public void UpdateRoomList(List<Data.Logic.Room> roomlist)
        {
            RoomListSerialization serRoomlist = new RoomListSerialization();
            foreach (Data.Logic.Room room in roomlist)
            {
                serRoomlist.Rooms.Add(new RoomSerialization(room, sessionAnchors));
            }
            saveLoadManager.SaveRoomList("rooms", serRoomlist);

        }
        */
        /*
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
        */
        /// <summary>
        /// Called on load button button click; changes state so user sees load slots.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick()
        {
            SwitchToState(State.Loading);
        }

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