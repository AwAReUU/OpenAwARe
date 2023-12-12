// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.Linq;
using AwARe.IngredientList.Logic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// An UI Element displaying and managing the single ingredient screen.
    /// </summary>
    public class IngredientScreen : MonoBehaviour
    {
        // The parent element
        [SerializeField] private IngredientListManager manager;
        
        // UI elements to control
        [SerializeField] private GameObject ingredientNameField;
        [SerializeField] private TMP_InputField quantityField;
        [SerializeField] private TMP_Dropdown typeDropdown;
        
        // The data represented/edited
        private Logic.IngredientList.Entry entry;
        private bool isNew; // TODO: Should be managed outside UI

        private void OnEnable()
        {
            entry = manager.SelectedEntry;
            isNew = manager.SelectedIsNew;

            SetElements();
        }

        /// <summary>
        /// Sets/corrects all UI elements on screen.
        /// </summary>
        private void SetElements()
        {
            // Set name label
            ingredientNameField.GetComponent<TMP_Text>().text = entry.ingredient.Name;
            
            // Set dropdown options
            List<string> options = Enum.GetNames(typeof(QuantityType)).ToList();
            typeDropdown.ClearOptions();
            typeDropdown.AddOptions(options);

            // Set default values
            quantityField.text = entry.quantity.ToString();
            typeDropdown.value = (int)entry.type;
        }

        /// <summary>
        /// Updates the current ingredient with values from inputs.
        /// </summary>
        public void OnConfirmClick()
        {
            Ingredient ingredient = entry.ingredient;
            string quantityS = quantityField.text;
            string typeS = typeDropdown.value.ToString();

            manager.ReadQuantity(ingredient, quantityS, typeS, out float quantity, out QuantityType type);

            if(isNew)
                manager.AddIngredient(ingredient, quantity, type);
            else
                manager.UpdateIngredient(quantity, type);

            manager.ChangeToIngredientListScreen(manager.SelectedList, this.gameObject);
        }

        /// <summary>
        /// Calls an instance of manager to close this screen and open the ingredientListScreen.
        /// </summary>
        public void OnBackButtonClick()
        {
            manager.ChangeToIngredientListScreen(manager.SelectedList, this.gameObject);
        }
    }
}