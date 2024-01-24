// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.UI.Objects;
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

        // The UI elements
        [SerializeField] private GameObject resetButton;
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject applyButton;
        [SerializeField] private GameObject confirmButton;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Pointer pointer;
        [SerializeField] private GameObject pathButton;
        [SerializeField] private GameObject pathLoadingPopup;
        [SerializeField] private GameObject saveButton;
        [SerializeField] private GameObject loadButton;
        [SerializeField] private GameObject saveList;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject saveNameButton;
        [SerializeField] private GameObject saveNameObject;
        [SerializeField] private GameObject roomlistscreen;
        [SerializeField] private TMP_InputField saveName;
        [SerializeField] private Button confirmName;


        /// <summary>
        /// Sets activity of UI elements based on the polygon state.
        /// </summary>
        /// <param name="roomState">Current/new room state.</param>
        /// <param name="polygonState">Current/new polygon state.</param>
        /// <param name="pathState">Current/new path state.</param>
        public void SetActive(State roomState, Polygons.State polygonState, Path.State pathState)
        {
            // Set all to inactive.
            bool reset = false,
                create = false,
                apply = false,
                confirm = false,
                height = false,
                point = false,
                pathGen = false,
                pathLoading = false,
                save = false,
                load = false,
                conti = false,
                savenamebtn = false,
                savelist = false,
                roomlist = false;

            // Set wanted elements to active
            void DecideActivities()
            {
                if (pathState == Path.State.Generating)
                {
                    pathLoading = true;
                    return;
                }

                //UnityEngine.Debug.Log(roomState.ToString());
                switch (roomState)
                {
                    case State.SaveAnchoring:
                        save = true;
                        conti = true;
                        point = true;
                        //startSaving = true;
                        return;
                    case State.LoadAnchoring:
                        load = true;
                        conti = true;
                        point = true;
                        //startLoading = true;
                        return;
                    case State.Saving:
                        conti = true;
                        savenamebtn = true;
                        roomlist = true;
                        return;
                    case State.Loading:
                        conti = true;
                        roomlist = true;
                        return;
                }

                switch (polygonState)
                {
                    case Polygons.State.Done:
                        create = true;
                        save = true;
                        load = true;
                        pathGen = true;
                        break;
                    case Polygons.State.SettingHeight:
                        height = true;
                        confirm = true;
                        break;
                    case Polygons.State.Drawing:
                        apply = true;
                        reset = true;
                        point = true;
                        break;
                    case Polygons.State.Default:
                    default:
                        load = true;
                        create = true;
                        break;
                }
            }
            DecideActivities();

            // Actual (de)activation.
            resetButton.SetActive(reset);
            createButton.SetActive(create);
            applyButton.SetActive(apply);
            confirmButton.SetActive(confirm);
            heightSlider.gameObject.SetActive(height);
            if (height) OnHeightSliderChanged();
            pointer.gameObject.SetActive(point);
            pathButton.SetActive(pathGen);
            roomlistscreen.SetActive(roomlist);
            pathLoadingPopup.SetActive(pathLoading);
            saveButton.SetActive(save);
            saveNameButton.SetActive(savenamebtn);
            saveList.SetActive(savelist);
            loadButton.SetActive(load);
            continueButton.SetActive(conti);
        }

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
        /// Called on apply button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnApplyButtonClick() =>
            manager.OnApplyButtonClick();

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

        [ExcludeFromCoverage]
        public void OnStartSavingButtonClick() =>
            manager.OnStartSavingButtonClick();

        /// <summary>
        /// Called on save button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick() =>
            manager.OnLoadButtonClick();

        [ExcludeFromCoverage]
        public void OnStartLoadingButtonClick() =>
            manager.OnStartLoadingButtonClick();

        /// <summary>
        /// Called on continue button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnContinueClick() =>
            manager.OnContinueClick();

        /// <summary>
        /// Called on path button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnPathButtonClick() =>
            manager.OnPathButtonClick();
    }
}
