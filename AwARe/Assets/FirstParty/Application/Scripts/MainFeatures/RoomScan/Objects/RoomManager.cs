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
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;
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
        private AnchorHandler anchorHandler;

        // The UI
        [SerializeField] private RoomUI ui;
        [SerializeField] private Transform canvas;
        [SerializeField] private Transform sceneCanvas;
        [SerializeField] private RoomOverviewScreen roomOverviewScreen;

        // Template
        [SerializeField] private GameObject roomBase;

        /// <summary>
        /// A serialized room list.
        /// </summary>
        public RoomListSerialization RoomListSerialization => roomListManager.GetSerRoomList();

        /// <summary>
        /// Gets or sets the current room; used for saving.
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// Serialized room; used for loading.
        /// </summary>
        public RoomSerialization SerRoom { get; set; }

        /// <summary>
        /// Gets the current state of the room scanner.
        /// </summary>
        /// <value>
        /// The current state of the room scanner.
        /// </value>
        public State CurrentState { get; private set; }

        /// <summary>
        /// The screenshots used for saving/loading rooms.
        /// </summary>
        private List<Texture2D> screenshots = new();

        [ExcludeFromCoverage]
        private void Awake()
        {
            anchorHandler = gameObject.GetComponent<AnchorHandler>();

            ui.gameObject.SetActive(true);

            // Move all content prefab canvas to scene canvas.
            Mover.MoveAllChildren(canvas, sceneCanvas, true);

            // Instantiate a room to construct.
            Room = Instantiate(roomBase, transform).GetComponent<Room>();

            SwitchToState(State.RoomList);
        }

        /// <summary>
        /// Called on create button click.
        /// </summary>
        public void OnCreateButtonClick()
        {
            if (CurrentState == State.AskForSave || CurrentState == State.RoomList)
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
            }
            else if (CurrentState == State.SaveAnchoringCheck)
            {
                roomListManager.screenshotManager.HideScreenshot();
                if (anchorHandler.SessionAnchors.Count >= anchorHandler.AnchorCount)
                {
                    SwitchToState(State.Saving);
                }
                else
                {
                    SwitchToState(State.SaveAnchoring);
                }
            }
            else if(CurrentState == State.AskForSave)
            {
                anchorHandler.SessionAnchors.Clear();
                screenshots.Clear();
                SwitchToState(State.SaveAnchoring);
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
                anchorHandler.TryAddAnchor();

                Texture2D screenshot = ui.screenshotManager.TakeScreenshot();
                screenshots.Add(screenshot);

                ui.DisplayAnchorSavingImage(screenshot);

                SwitchToState(State.SaveAnchoringCheck);
            }
            else if (CurrentState == State.LoadAnchoring)
            {
                anchorHandler.TryAddAnchor();
                screenshots.Add(ui.screenshotManager.TakeScreenshot());

                if (anchorHandler.SessionAnchors.Count < anchorHandler.AnchorCount)
                    ui.DisplayAnchorLoadingImage(anchorHandler.SessionAnchors.Count);
                else
                {
                    Data.Logic.Room room = roomListManager.LoadRoom(SerRoom, anchorHandler.SessionAnchors);
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
                anchorHandler.TryRemoveLastAnchor();
                screenshots.RemoveAt(screenshots.Count - 1);
                ui.HideScreenshot();
                SwitchToState(State.SaveAnchoring);
            }
            else if (polygonManager.CurrentState == Polygons.State.AskForNegPolygons)
            {
                SwitchToState(State.AskForSave);
            }
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
            anchorHandler.SessionAnchors.Clear();
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
        public void SaveRoom(string roomName)
        {
            Room.roomName = roomName;
            roomListManager.SaveRoom(Room.Data, anchorHandler.SessionAnchors, screenshots);

            roomOverviewScreen.DisplayList();
            
            GoToRoom(Room.Data);
        }

        /// <summary>
        /// Deletes the data and screenshots of the given room.
        /// </summary>
        /// <param name="roomIndex">The index of the room that is being deleted.</param>
        public void DeleteRoom(int roomIndex)
        {
            roomListManager.DeleteRoom(roomIndex, anchorHandler.AnchorCount);
            roomOverviewScreen.DisplayList();
        }
        
        public bool IsFirstPolygon() =>
            polygonManager.IsFirstPolygon();

        /// <summary>
        /// Called on load button button click; changes state so user sees load slots.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick()
        {
            SwitchToState(State.RoomList);
        }

        /// <summary>
        /// Called on path button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnPathButtonClick() =>
            pathManager.OnPathButtonClick();

        /// <summary>
        /// Checks if room name already exists in the rooms file;
        /// if not then it will be saved and will show up in the list of roomsaves.
        /// </summary>
        public void OnConfirmNameButtonClick(string roomName)
        {
            
            if (RoomListSerialization.Rooms.Where(obj => obj.RoomName == roomName).Count() > 0)
                Debug.LogError("This name already exists");
            else
            {
                SaveRoom(roomName);
            }
        }

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
            // Set UI activity
            ui.SetActive(this.CurrentState, polygonManager.CurrentState, pathManager.CurrentState);
        }
    }

    /// <summary>
    /// The different states within the Room scanning process.
    /// </summary>
    public enum State
    {
        Scanning,
        AskForSave,
        Saving,
        RoomList,
        SaveAnchoring,
        SaveAnchoringCheck,
        LoadAnchoring
    }
}