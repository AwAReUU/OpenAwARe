using System;
using System.Collections.Generic;

using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    public class SearchItem : MonoBehaviour
    {
        
        private Ingredient ingredient;

        // (assigned within unity)
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private SearchScreen screen;

        /// <summary>
        /// TODO
        /// </summary>
        public void SetItem(Ingredient ingredient)
        {
            this.ingredient = ingredient;
            DisplayItem();
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        private void DisplayItem()
        {
            gameObject.name =  ingredient.Name;
            nameLabel.text = ingredient.Name;
        }

        public void OnItemClick() =>
            screen.OnItemClick(ingredient);
    }
}