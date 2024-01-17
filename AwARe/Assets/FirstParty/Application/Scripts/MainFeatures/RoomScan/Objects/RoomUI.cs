// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;

using AwARe.UI;
using AwARe.UI.Objects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// UI in the polygon scan.
    /// </summary>
    public class RoomUI : MonoBehaviour, IPointer
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
        [SerializeField] private GameObject saveSlots;
        [SerializeField] private GameObject loadButton;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private GameObject saveNameObject;
        [SerializeField] private TMP_InputField saveName;
       


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
                saveSlots = false,
                conti = false;

            // Set wanted elements to active
            void DecideActivities()
            {
                if (pathState == Path.State.Generating)
                {
                    pathLoading = true;
                    return;
                }

                switch (roomState)
                {
                    case State.Saving:
                        conti = true;
                        saveSlots = true;
                        return;
                    case State.Loading:
                        conti = true;
                        saveSlots = true;
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
            pathLoadingPopup.SetActive(pathLoading);
            saveButton.SetActive(save);
            this.saveSlots.SetActive(saveSlots);
            loadButton.SetActive(load);
            continueButton.SetActive(conti);
        }

        /// <summary>
        /// Gets the current position of the pointer.
        /// </summary>
        /// <value>
        /// The current position of the pointer.
        /// </value>
        public virtual Vector3 PointedAt =>
            pointer.PointedAt;

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

        /// <summary>
        /// Called on save slot click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnSaveSlotClick(int slotIdx) =>
            manager.OnSaveSlotClick(slotIdx);

        /// <summary>
        /// Called on save button click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadButtonClick() =>
            manager.OnLoadButtonClick();

        /// <summary>
        /// Called on load slot click.
        /// </summary>
        [ExcludeFromCoverage]
        public void OnLoadSlotClick(int slotIdx) =>
            manager.OnLoadSlotClick(slotIdx);

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
