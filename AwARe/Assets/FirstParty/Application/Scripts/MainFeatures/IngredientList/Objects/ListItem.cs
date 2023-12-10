using System;
using System.Collections.Generic;

using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField] private ListsOverviewScreen screen;

        private Logic.IngredientList list;
        private int index;

        // (assigned within unity)
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI quantityLabel;
        [SerializeField] private Image checkButton;
        [SerializeField] private GameObject checkHighlight;
        [SerializeField] private Color checkOnColor;
        private Color checkOffColor;

        private void Awake()
        {
            checkOffColor = checkButton.color;
        }

        /// <summary>
        /// TODO
        /// </summary>
        public void SetItem(int index, Logic.IngredientList list)
        {
            this.index = index;
            this.list = list;
            DisplayItem();
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name =  list.ListName;
            nameLabel.text = list.ListName;
            quantityLabel.text = list.NumberOfIngredients().ToString();
        }

        public void Check(bool check)
        {
            if (check)
            {
                checkButton.color = checkOnColor;
                checkHighlight.SetActive(true);
            }
            else
            {
                checkButton.color = checkOffColor;
                checkHighlight.SetActive(false);
            }
        }

        public void OnCheckButtonClick() =>
            screen.OnCheckButtonClick(index, list);

        public void OnItemClick() =>
            screen.OnItemClick(list);
    }
}