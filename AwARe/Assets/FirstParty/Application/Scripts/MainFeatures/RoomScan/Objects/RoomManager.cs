// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Logic;
using AwARe.Data.Objects;
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
        /// Gets a serialized room list.
        /// </summary>
        /// <value>
        /// A serialized room list.
        /// </value>
        public RoomListSerialization RoomListSerialization => roomListManager.GetSerRoomList();

        /// <summary>
        /// Gets or sets the current room; used for saving.
        /// </summary>
        /// <value>
        /// >The current room; used for saving.
        /// </value>
        public Room Room { get; set; }

        /// <summary>
        /// Gets or sets the serialized room; used for loading.
        /// </summary>
        /// <value>
        /// Serialized room; used for loading.
        /// </value>
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
        private readonly List<Texture2D> screenshots = new();

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
        /// Starts the process of loading the room.
        /// </summary>
        /// <param name="roomIndex">The index of the desired room in the serialized room list.</param>
        public void StartLoadingRoom(int roomIndex)
        {
            SerRoom = roomListManager.GetSerRoomList().Rooms[roomIndex];
            anchorHandler.SessionAnchors.Clear();
            screenshots.Clear();
            SwitchToState(State.LoadAnchoring);
        }

        /// <summary>
        /// Go to the AR scene with the desired room active.
        /// </summary>
        /// <param name="room">The room to be active in the AR scene.</param>
        public void GoToRoom(Data.Logic.Room room)
        {
            Storage.Get().ActiveRoom = room;
            Storage.Get().ActivePath = pathManager.GenerateAndDrawPath();
            SceneSwitcher.Get().LoadScene("AR");
        }

        /// <summary>
        /// Save newly created room in rooms file.
        /// </summary>
        /// <param name="roomName">The name of the Room.</param>
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
            roomListManager.DeleteRoom(roomIndex, anchorHandler.anchorCount);
            roomOverviewScreen.DisplayList();
        }

        /// <summary>
        /// Whether the polygon being drawn is the positive polygon.
        /// </summary>
        /// <returns>Whether positivePolygon is null (meaning no polygon has been added to the room yet).</returns>
        public bool IsFirstPolygon() =>
            polygonManager.IsFirstPolygon();

        /// <summary>
        /// Called on create button click.
        /// </summary>
        public void OnCreateButtonClick()
        {
            if (CurrentState == State.AskToSave || CurrentState == State.RoomList)
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
            switch (CurrentState)
            {
                case State.Scanning:
                    polygonManager.OnConfirmButtonClick();
                    break;
                case State.AskToSave:
                    // user wants to save the room; emtpy lists and start the anchoring process
                    anchorHandler.SessionAnchors.Clear();
                    screenshots.Clear();
                    SwitchToState(State.SaveAnchoring);
                    break;
                case State.SaveAnchoringCheck:
                    // anchor accepted; hide screenshot and either continue with next or finish anchoring
                    ui.HideScreenshot();
                    if (anchorHandler.AnchoringFinished())
                        SwitchToState(State.InputtingName);
                    else
                        SwitchToState(State.SaveAnchoring);
                    break;
            }
        }

        /// <summary>
        /// Called on the select button click; Places points/anchors.
        /// </summary>
        public void OnSelectButtonClick()
        {
            switch (CurrentState)
            {
                case State.Scanning:
                    // Add a point to the polygon
                    polygonManager.TryAddPoint();
                    break;
                case State.SaveAnchoring:
                    // Add an anchor and take a screenshot
                    anchorHandler.TryAddAnchor();
                    Texture2D screenshot = ui.screenshotManager.TakeScreenshot();
                    screenshots.Add(screenshot);

                    // Display the taken screenshot
                    ui.DisplayAnchorSavingImage(screenshot);

                    SwitchToState(State.SaveAnchoringCheck);
                    break;
                case State.LoadAnchoring:
                    // Add an anchor
                    anchorHandler.TryAddAnchor();

                    if (!anchorHandler.AnchoringFinished())
                        // Load the next screenshot
                        ui.DisplayAnchorLoadingImage(anchorHandler.SessionAnchors.Count);
                    else
                    {
                        // Finished placing anchors; load in the room
                        Data.Logic.Room room = roomListManager.LoadRoom(SerRoom, anchorHandler.SessionAnchors);
                        GoToRoom(room);
                    }
                    break;
            }
        }

        /// <summary>
        /// Called on 'No' button click.
        /// </summary>
        public void OnNoButtonClick()
        {
            switch (CurrentState)
            {
                case State.Scanning:
                    if(polygonManager.CurrentState == Polygons.State.AskForNegPolygons)
                        // The user does not want to place a negative polygon; stop scanning
                        SwitchToState(State.AskToSave);
                    break;
                case State.AskToSave:
                    // The user does not want to save; go to AR scene
                    GoToRoom(Room.Data);
                    break;
                case State.SaveAnchoringCheck:
                    // The point was not recognizable; revert placing the anchor
                    anchorHandler.TryRemoveLastAnchor();
                    screenshots.RemoveAt(screenshots.Count - 1);
                    ui.HideScreenshot();
                    SwitchToState(State.SaveAnchoring);
                    break;
            }
        }

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        /// <param name="value">The value of the slider.</param>
        [ExcludeFromCoverage]
        public void OnHeightSliderChanged(float value) =>
            polygonManager.OnHeightSliderChanged(value);

        /// <summary>
        /// Checks if room name already exists in the rooms file;
        /// if not then it will be saved and will show up in the list of roomsaves.
        /// </summary>
        /// <param name="roomName">The name of the room.</param>
        public void OnConfirmNameButtonClick(string roomName)
        {
            if (RoomListSerialization.Rooms.Where(obj => obj.RoomName == roomName).Count() > 0)
                Debug.LogError("This name already exists");
            else
                SaveRoom(roomName);
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
        [ExcludeFromCoverage]
        public void SetActive()
        {
            // Set UI activity
            ui.SetActive(this.CurrentState, polygonManager.CurrentState, pathManager.CurrentState);
        }
    }

    /// <summary>
    /// The different states within the Room scanning and loading process.
    /// </summary>
    public enum State
    {
        RoomList,           // shows the list with rooms
        Scanning,           // in the scanning process
        AskToSave,          // question whether the user wants to save the room
        InputtingName,      // inputting name for the room being saved
        SaveAnchoring,      // placing anchors for saving a room
        SaveAnchoringCheck, // question if the placed anchor is correct
        LoadAnchoring       // placing anchors for loading a room
    }
}