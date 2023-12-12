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

        /// <summary>
        /// Sets activity of UI elements based on the state.
        /// </summary>
        /// <param name="toState">Current/new state.</param>
        public void SetActive(State state)
        {
            bool reset = false, create = false, apply = false,
                confirm = false, save = false, height = false,
                point = false;
            switch (state)
            {
                case State.Saving:
                    create = true;
                    save = true;
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
            heightSlider.SetActive(height);
            pointer.SetActive(point);
        }
    }
}
