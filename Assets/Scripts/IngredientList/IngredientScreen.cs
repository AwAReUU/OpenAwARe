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
        /// Initialize the quantityTypeDropdown
        /// </summary>
        private void SetDropDownItems()
        {
            //convert QuantityType names to string list
            List<string> dropOptions = Enum.GetNames(typeof(QuantityType)).ToList();
            TMP_Dropdown dropdown = qtyTypeDropdown.GetComponent<TMP_Dropdown>();
            dropdown.ClearOptions();
            dropdown.AddOptions(dropOptions);

            //set selected value to current
            dropdown.value = (int)ingredientListManager.SelectedList.GetQuantityType(selectedIngredient);
        }

        /// <summary>
        /// Updates the current ingredient with values from inputs
        /// </summary>
        public void OnConfirmClick()
        {
            string qInput = qtyInput.GetComponent<TMP_InputField>().text;
            string qTypeInput = qtyTypeDropdown.GetComponent<TMP_Dropdown>().value.ToString();

            if (!Enum.TryParse(qTypeInput, out QuantityType parsedQType))
            {
                throw new Exception("Cannot convert string to QuantityType.");
            }

            if ((parsedQType == QuantityType.ML && !ingredientListManager.SelectedIngredient.MLQuantityPossible())
                || parsedQType == QuantityType.PCS && !ingredientListManager.SelectedIngredient.PieceQuantityPossible())
            {
                throw new Exception("Chosen QuantityType not available for this ingredient.");
            }

            float parsedQty;
            if (float.TryParse(qInput, out parsedQty))
            {
                UpdateIngredient(parsedQType, parsedQty);
                ingredientListManager.OpenList(ingredientListManager.SelectedList, this.gameObject);
            }
            else
            {
                //show some error to user
            }
        }

        private void UpdateIngredient(QuantityType type, float newQuantity)
        {
            ingredientListManager.SelectedList.Ingredients[ingredientListManager.SelectedIngredient] = (newQuantity, type);
        }

        /// <summary>
        /// Go back to previous screen
        /// </summary>
        private void OnBackButtonClick()
        {
            ingredientListManager.OpenList(ingredientListManager.SelectedList, this.gameObject);
        }
    }
}