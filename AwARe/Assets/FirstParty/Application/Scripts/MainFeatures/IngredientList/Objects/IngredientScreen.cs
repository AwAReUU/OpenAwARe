using System;
using System.Collections.Generic;
using System.Linq;

using AwARe.IngredientList.Logic;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AwARe.IngredientList.Objects
{
    public class IngredientScreen : MonoBehaviour
    {
        [FormerlySerializedAs("ingredientListManager")][SerializeField] private IngredientListManager manager;

        [SerializeField] private GameObject ingredientNameField;
        [SerializeField] private TMP_InputField quantityField;
        [SerializeField] private TMP_Dropdown typeDropdown;

        private Logic.IngredientList.Entree entree;
        private bool isNew;

        private void OnEnable()
        {
            entree = manager.SelectedEntree;
            isNew = manager.SelectedIsNew;

            SetElements();
        }

        /// <summary>
        /// Initializes the quantityTypeDropdown
        /// </summary>
        private void SetElements()
        {
            // Set name label
            ingredientNameField.GetComponent<TMP_Text>().text = entree.ingredient.Name;
            
            // Set dropdown options
            List<string> options = Enum.GetNames(typeof(QuantityType)).ToList();
            typeDropdown.ClearOptions();
            typeDropdown.AddOptions(options);

            // Set default values
            quantityField.text = entree.quantity.ToString();
            typeDropdown.value = (int)entree.type;
        }

        /// <summary>
        /// Updates the current ingredient with values from inputs
        /// </summary>
        public void OnConfirmClick()
        {
            Ingredient ingredient = entree.ingredient;
            string quantityS = quantityField.text;
            string typeS = typeDropdown.value.ToString();

            manager.ConvertToQuantity(ingredient, quantityS, typeS, out float quantity, out QuantityType type);
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