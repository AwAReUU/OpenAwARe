// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.UI.Objects;
using UnityEngine;
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
        [SerializeField] private GameObject saveButton;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private Pointer pointer;
        [SerializeField] private GameObject pathButton;
        [SerializeField] private GameObject loadingPopup;

        /// <summary>
        /// Sets activity of UI elements based on the polygon state.
        /// </summary>
        /// <param name="polygonState">Current/new polygon state.</param>
        /// <param name="pathState">Current/new path state.</param>
        public void SetActive(Polygons.State polygonState, Path.State pathState)
        {
            // Set all to inactive.
            bool reset = false, create = false, apply = false,
                confirm = false, save = false, height = false,
                point = false, pathGen = false, loading = false;
            
            // Set wanted elements to active
            void DecideActivities()
            {
                switch (pathState)
                {
                    case Path.State.Generating:
                        loading = true;
                        return;
                    case Path.State.Default:
                    default:
                        break;
                }

                switch (polygonState)
                {
                    case Polygons.State.Saving:
                        create = true;
                        save = true;
                        pathGen = true;
                        break;
                    case Polygons.State.SettingHeight:
                        height = true;
                        confirm = true;
                        break;
                    case Polygons.State.Scanning:
                        apply = true;
                        reset = true;
                        point = true;
                        break;
                    case Polygons.State.Default:
                    default:
                        create = true;
                        break;
                }
            };
            DecideActivities();

            // Actual (de)activation.
            resetButton.SetActive(reset);
            createButton.SetActive(create);
            applyButton.SetActive(apply);
            confirmButton.SetActive(confirm);
            saveButton.SetActive(save);
            heightSlider.gameObject.SetActive(height);
            pointer.gameObject.SetActive(point);
            pathButton.SetActive(pathGen);
            loadingPopup.SetActive(loading);
        }

        /// <summary>
        /// Gets the current position of the pointer.
        /// </summary>
        /// <value>
        /// The current position of the pointer.
        /// </value>
        public Vector3 PointedAt =>
            pointer.transform.position;

        /// <summary>
        /// Called on create button click.
        /// </summary>
        public void OnCreateButtonClick() =>
            manager.OnCreateButtonClick();


        /// <summary>
        /// Called on reset button click.
        /// </summary>
        public void OnResetButtonClick() =>
            manager.OnResetButtonClick();


        /// <summary>
        /// Called on apply button click.
        /// </summary>
        public void OnApplyButtonClick() =>
            manager.OnApplyButtonClick();

        /// <summary>
        /// Called on confirm button click.
        /// </summary>
        public void OnConfirmButtonClick() =>
            manager.OnConfirmButtonClick();

        /// <summary>
        /// Called on changing the slider.
        /// </summary>
        public void OnHeightSliderChanged() =>
            manager.OnHeightSliderChanged(heightSlider.value);

        /// <summary>
        /// Called on save button click.
        /// </summary>
        public void OnSaveButtonClick() =>
            manager.OnSaveButtonClick();
        
        /// <summary>
        /// Called on path button click.
        /// </summary>
        public void OnPathButtonClick() =>
            manager.OnPathButtonClick();
    }
}
