using System;
using System.Collections;
using System.Collections.Generic;

using AwARe.RoomScan.Polygons.Objects;

using UnityEngine;
using UnityEngine.UI;

namespace AwARe.RoomScan.Polygons.Objects
{
    public class PolygonUI : MonoBehaviour
    {
        [SerializeField] private GameObject resetButton;
        [SerializeField] private GameObject createButton;
        [SerializeField] private GameObject applyButton;
        [SerializeField] private GameObject confirmButton;
        [SerializeField] private GameObject saveButton;
        [SerializeField] private GameObject heightSlider;
        [SerializeField] private GameObject pointer;
        [SerializeField] private GameObject pathButton;
        [SerializeField] private GameObject loadingpopup;
        [SerializeField] private GameObject saveButtons;
        [SerializeField] private GameObject loadButtons;
        [SerializeField] private GameObject loadButton;

        /// <summary>
        /// Sets activity of UI elements based on the state.
        /// </summary>
        /// <param name="toState">Current/new state.</param>
        public void SetActive(State state)
        {
            bool reset = false, create = false, apply = false,
                confirm = false, save = false, height = false,
                point = false, pathbutton = false, Loadingpopup = false, savebtns= false;
            switch (state)
            {
                case State.Saving:
                    create = true;
                    save = true;
                    pathbutton = true;
                    break;
                case State.SavingOptions:
                    savebtns = true;
                    break;
                case State.Scanning:
                    apply = true;
                    reset = true;
                    point = true;
                    break;
                case State.SettingHeight:
                    height = true;
                    confirm = true;
                    break;
                case State.Default:
                default:
                    create = true;
                    break;
               

            }
            saveButtons.SetActive(savebtns);
            resetButton.SetActive(reset);
            createButton.SetActive(create);
            applyButton.SetActive(apply);
            confirmButton.SetActive(confirm);
            saveButton.SetActive(save);
            saveButtons.SetActive(savebtns);
            heightSlider.SetActive(height);
            pointer.SetActive(point);
            pathButton.SetActive(pathbutton);
            loadingpopup.SetActive(Loadingpopup);
            
        }
    }
}
