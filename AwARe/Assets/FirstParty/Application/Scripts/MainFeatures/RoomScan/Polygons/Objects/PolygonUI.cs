// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \* 
using System;
using System.Collections;
using System.Collections.Generic;

using AwARe.RoomScan.Polygons.Logic;
using AwARe.RoomScan.Polygons.Objects;

using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private GameObject savedpopup;
        [SerializeField] private GameObject saveButtons;
        [SerializeField] private GameObject anchorButtons;
        [SerializeField] private GameObject loadButtons;
        [SerializeField] private GameObject loadButton;
        [SerializeField] private GameObject continueButton;

        /// <summary>
        /// Sets activity of UI elements based on the state.
        /// </summary>
        /// <param name="state">Current/new state.</param>
        public void SetActive(State state)
        {
            bool reset = false,
                create = false,
                apply = false,
                confirm = false,
                save = false,
                height = false,
                anchor1btn = false,
                point = false,
                pathbutton = false,
                Loadingpopup = false,
                savebtns = false,
                load = false,
                loadbtns = false,
                continuebtn = false;
            switch (state)
            {
                case State.Saving:
                    create = true;
                    save = true;
                    pathbutton = true;
                    anchor1btn = true;
                    break;
                case State.SettingAnchors:
                    //save = true;
                    anchor1btn = true;
                    point = true;
                    break;
                case State.SavingOptions:
                    savebtns = true;
                    break;
                case State.Loading:
                    load = true;
                    break;
                case State.LoadingOptions:
                    loadbtns = true;
                    continuebtn = true;
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

            resetButton.SetActive(reset);
            createButton.SetActive(create);
            applyButton.SetActive(apply);
            confirmButton.SetActive(confirm);
            saveButton.SetActive(save);
            saveButtons.SetActive(savebtns);
            loadButton.SetActive(load);
            loadButtons.SetActive(loadbtns);
            anchorButtons.SetActive(anchor1btn);
            heightSlider.SetActive(height);
            pointer.SetActive(point);
            pathButton.SetActive(pathbutton);
            loadingpopup.SetActive(Loadingpopup);
            continueButton.SetActive(continuebtn);
            
            Debug.Log(state);
            
        }
    }
}
