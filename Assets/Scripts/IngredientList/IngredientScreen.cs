using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IngredientLists
{
    public class IngredientScreen : MonoBehaviour
    {
        [SerializeField] private IngredientListManager ingredientListManager;

        [SerializeField] private GameObject ingredientNameField;
        [SerializeField] private GameObject backButton;
        [SerializeField] private GameObject confirmButton;
        [SerializeField] private GameObject qtyInput;
        [SerializeField] private GameObject qtyTypeDropdown;

        private Ingredient selectedIngredient;
        private float selectedIngredientQuantity;
        private QuantityType selectedIngredientQType;
        

        void OnEnable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.AddListener(delegate { OnBackButtonClick(); });

            selectedIngredient = ingredientListManager.SelectedIngredient;
            selectedIngredientQuantity = ingredientListManager.SelectedList.GetQuantity(selectedIngredient);
            selectedIngredientQType = ingredientListManager.SelectedList.GetQuantityType(selectedIngredient);

            ingredientNameField.GetComponent<TMP_Text>().text = selectedIngredient.Name;
            qtyInput.GetComponent<TMP_InputField>().text = selectedIngredientQuantity.ToString();

            SetDropDownItems();
        }
        private void OnDisable()
        {
            Button backB = backButton.GetComponent<Button>();
            backB.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// Initializes the quantityTypeDropdown
        /// </summary>
        private void SetDropDownItems()
        {
            //convert QuantityType names to string list
            List<string> dropOptions = Enum.GetNames(typeof(QuantityType)).ToList();
            TMP_Dropdown dropdown = qtyTypeDropdown.GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(dropOptions);

            //set selected value to current
            dropdown.value = (int)selectedIngredientQType;
;
        }


        /// <summary>
        /// Updates the current ingredient with values from inputs
        /// </summary>
        public void OnConfirmClick()
        {
            string qInput = qtyInput.GetComponent<TMP_InputField>().text;
            string qTypeInput = qtyTypeDropdown.GetComponent<TMP_Dropdown>().value.ToString();

            // try converting the quantity type string to a QuantityType
            if (!Enum.TryParse(qTypeInput, out QuantityType parsedQType))
            {
                throw new Exception("Cannot convert string to QuantityType.");
            }

            // check whether the quantity type is valid for this ingredient
            if ((parsedQType == QuantityType.ML && !ingredientListManager.SelectedIngredient.MLQuantityPossible())
                || parsedQType == QuantityType.PCS && !ingredientListManager.SelectedIngredient.PieceQuantityPossible())
            {
                throw new Exception("Chosen QuantityType not available for this ingredient.");
            }

            // try converting the quantity string to a Quantity
            if (!float.TryParse(qInput, out float parsedQty))
            {
                throw new Exception("Cannot convert string to Quantity.");
            }

            ingredientListManager.UpdateIngredient(parsedQty, parsedQType);
            ingredientListManager.ChangeToIngredientListScreen(ingredientListManager.SelectedList, this.gameObject);
            
        }

        /// <summary>
        /// Calls an instance of ingredientListManager to close this screen and open the ingredientListScreen.
        /// </summary>
        private void OnBackButtonClick()
        {
            ingredientListManager.ChangeToIngredientListScreen(ingredientListManager.SelectedList, this.gameObject);
        }
    }
}