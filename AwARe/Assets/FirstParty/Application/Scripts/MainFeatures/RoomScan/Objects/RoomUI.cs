// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.RoomScan.Polygons.Objects;
using AwARe.UI.Objects;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// UI in the polygon scan.
    /// </summary>
    public class RoomUI : MonoBehaviour
    {
        // The manager
        [SerializeField] private RoomManager manager;

        // The screenshot manager
        [SerializeField] public ScreenshotManager screenshotManager;

        // The UI elements
        [SerializeField] private GameObject resetButton;
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject confirmButton;
        [SerializeField] private GameObject noButton;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Pointer pointer;
        [SerializeField] private GameObject pathLoadingPopup;
        [SerializeField] private GameObject saveButton;
        [SerializeField] private GameObject selectPointButton;
        [SerializeField] private GameObject roomlistscreen;
        [SerializeField] private GameObject nameInputWindow;
        [SerializeField] public TMP_InputField nameInput;
        [SerializeField] private GameObject findPointText;
        [SerializeField] private GameObject askForSaveText;
        [SerializeField] private GameObject placeAnchorText;
        [SerializeField] private GameObject anchorRecognizableText;
        [SerializeField] private GameObject askForNegPolygonsText;
        [SerializeField] private GameObject setRoomHeightText;
        [SerializeField] private GameObject setObstacleHeightText;


        /// <summary>
        /// Sets activity of UI elements based on the polygon state.
        /// </summary>
        /// <param name="roomState">Current/new room state.</param>
        /// <param name="polygonState">Current/new polygon state.</param>
        /// <param name="pathState">Current/new path state.</param>
        public void SetActive(State roomState, Polygons.State polygonState, Path.State pathState)
        {
            Debug.Log("Roomstate: " + roomState);
            Debug.Log("PolyState: " + roomState);

            // Set all to inactive.
            bool resetBtn = false,
                createBtn = false,
                confirmBtn = false,
                heightSlider = false,
                pointer = false,
                pathPopup = false,
                saveBtn = false,
                roomlist = false,
                displayScreenshot = false,
                noBtn = false,
                nameInputWin = false,
                findPointText = false,
                placeText = false,
                askSaveText = false,
                anchorRecogText = false,
                negPolygonsText = false,
                roomHeightText = false,
                obstacleHeightText = false;

            // Set wanted elements to active
            void DecideActivities()
            {
                if (pathState == Path.State.Generating)
                {
                    pathPopup = true;
                    return;
                }

                //UnityEngine.Debug.Log(roomState.ToString());
                switch (roomState)
                {
                    case State.SaveAnchoring:
                        pointer = true;
                        placeText = true;
                        return;
                    case State.SaveAnchoringCheck:
                        confirmBtn = true;
                        anchorRecogText = true;
                        noBtn = true;
                        return;
                    case State.AskForSave:
                        saveBtn = true;
                        noBtn = true;
                        askSaveText = true;
                        return;
                    case State.LoadAnchoring:
                        pointer = true;
                        findPointText = true;
                        displayScreenshot = true;
                        return;
                    case State.Saving:
                        nameInputWin = true;
                        return;
                    case State.Loading:
                        roomlist = true;
                        createBtn = true;
                        return;
                }

                switch (polygonState)
                {
                    case Polygons.State.SettingHeight:
                        heightSlider = true;
                        confirmBtn = true;
                        if(manager.IsFirstPolygon())
                            roomHeightText = true;
                        else
                            obstacleHeightText = true;
                        break;
                    case Polygons.State.Drawing:
                        confirmBtn = true;
                        resetBtn = true;
                        pointer = true;
                        break;
                    case Polygons.State.AskForNegPolygons:
                        negPolygonsText = true;
                        confirmBtn = true;
                        noBtn = true;
                        break;
                    case Polygons.State.Default:
                    default:
                        createBtn = true;
                        break;
                }
            }
            DecideActivities();

            // Actual (de)activation.
            resetButton.SetActive(resetBtn);
            createButton.SetActive(createBtn);
            confirmButton.SetActive(confirmBtn);
            this.heightSlider.gameObject.SetActive(heightSlider);
            if (heightSlider) OnHeightSliderChanged();
            this.pointer.gameObject.SetActive(pointer);
            roomlistscreen.SetActive(roomlist);
            pathLoadingPopup.SetActive(pathPopup);
            saveButton.SetActive(saveBtn);
            noButton.SetActive(noBtn);
            nameInputWindow.SetActive(nameInputWin);
            this.findPointText.SetActive(findPointText);
            selectPointButton.SetActive(pointer);
            askForSaveText.SetActive(askSaveText);
            placeAnchorText.SetActive(placeText);
            anchorRecognizableText.SetActive(anchorRecogText);
            askForNegPolygonsText.SetActive(negPolygonsText);
            setRoomHeightText.SetActive(roomHeightText);
            setObstacleHeightText.SetActive(obstacleHeightText);

            if (displayScreenshot)
                DisplayAnchorLoadingImage(0);
        }

        /// <summary>
        /// Display the screenshot with the given index for loading the anchors.
        /// </summary>
        /// <param name="index">The index of the screenshot.</param>
        public void DisplayAnchorLoadingImage(int index)
        {
            screenshotManager.DisplayScreenshotFromFile(manager.SerRoom.RoomName, index, false, ScreenshotManager.ImageSize.Small);
        }


        /// <summary>
        /// Display the screenshot with the given index for saving the anchors.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        public void DisplayAnchorSavingImage(Texture2D screenshot)
        {
            screenshotManager.DisplayScreenshot(screenshotManager.TextureToSprite(screenshot), false);
        }

        public void HideScreenshot() =>
            screenshotManager.HideScreenshot();

        /// <summary>
        /// Called on create button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnCreateButtonClick() =>
            manager.OnCreateButtonClick();


        /// <summary>
        /// Called on reset button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnResetButtonClick() =>
            manager.OnResetButtonClick();

        /// <summary>
        /// Called on select point button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnSelectButtonClick() =>
            manager.OnSelectButtonClick();

        /// <summary>
        /// Called on confirm button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnConfirmButtonClick() =>
            manager.OnConfirmButtonClick();

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnHeightSliderChanged() =>
            manager.OnHeightSliderChanged(heightSlider.value);

        /// <summary>
        /// Called on save button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnSaveButtonClick() =>
            manager.OnSaveButtonClick();

        /// <summary>
        /// Called on load button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick() =>
            manager.OnLoadButtonClick();

        /// <summary>
        /// Called on no button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnNoButtonClick() =>
            manager.OnNoButtonClick();

        /// <summary>
        /// Called on confirm name button click.
        /// </summary>
        public void OnConfirmNameButtonClick() =>
            manager.OnConfirmNameButtonClick(nameInput.text);
    }
}
