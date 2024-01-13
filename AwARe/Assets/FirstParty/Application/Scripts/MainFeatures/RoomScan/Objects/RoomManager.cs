// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.RoomScan.Path.Objects;

using UnityEngine;
using UnityEngine.TestTools;
using AwARe.UI;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomManager : MonoBehaviour, IPointer
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

        [ExcludeFromCoverage]
        private void Awake()
        {
            // Move all content prefab canvas to scene canvas.
            Mover.MoveAllChildren(canvas, sceneCanvas, true);

            // Instantiate a room to construct.
            Room = Instantiate(roomBase, transform).GetComponent<Room>();
        }

        /// <summary>
        /// Gets or sets the current room.
        /// </summary>
        /// <value>
        /// The current room.
        /// </value>
        public Room Room { get; set; }

        /// <summary>
        /// Gets the current position of the pointer.
        /// </summary>
        /// <value>
        /// The current position of the pointer.
        /// </value>
        public Vector3 PointedAt =>
            ui.PointedAt;

        /// <summary>
        /// Called when no UI element has been hit on click or press.
        /// </summary>
        public void OnUIMiss() =>
            polygonManager.OnUIMiss();

        /// <summary>
        /// Called on create button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnCreateButtonClick() =>
            polygonManager.OnCreateButtonClick();


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
        public void OnConfirmButtonClick() =>
            polygonManager.OnConfirmButtonClick();

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
            SceneSwitcher.Get().LoadScene("Home");
        }

        /// <summary>
        /// Called on path button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnPathButtonClick() =>
            pathManager.OnPathButtonClick();

        /// <summary>
        /// Sets activity of components.
        /// </summary>
        /// <param name="state">Current/new state.</param>
        [ExcludeFromCoverage]
        public void SetActive() =>
            // Set UI activity
            ui.SetActive(polygonManager.CurrentState, pathManager.CurrentState);
    }
}