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
using NSubstitute;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the Ingredient screen.
    /// </para>
    /// <para>
    ///     Displays the name and quantity of the currently selected <see cref="Ingredient"/>.
    ///     Allows the user to adjust this ingredient's quantity and QuantityType through UI elements.
    /// </para>
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
        /// Initializes the quantityTypeDropdown.
        /// </summary>
        private void SetElements()
        {
            // Set name label
            ingredientNameField.GetComponent<TMP_Text>().text = entry.ingredient.PrefName;

            typeDropdown.ClearOptions();

            // Get all valid dropdown options for the ingredient
            List<string> options = Enum.GetValues(typeof(QuantityType)).Cast<QuantityType>()
                .Where(qType => entry.ingredient.QuantityPossible(qType))
                .Select(t => t.ToString()).ToList();

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
            string typeS = typeDropdown.options[typeDropdown.value].text;

            manager.ReadQuantity(ingredient, quantityS, typeS, out float quantity, out QuantityType type);

            if (isNew)
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