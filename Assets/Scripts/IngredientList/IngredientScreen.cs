using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientScreen : MonoBehaviour
{
    [SerializeField] private IngredientListManager ingredientListManager;

    [SerializeField] private GameObject ingredientNameField;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject qtyInput;
    [SerializeField] private GameObject qtyTypeDropdown;

    private Ingredient currentIngredient;

    void OnEnable()
    {
        currentIngredient = ingredientListManager.SelectedIngredient;

        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        currentIngredient = ingredientListManager.SelectedIngredient;
        
        ingredientNameField.GetComponent<TMP_Text>().text = currentIngredient.Name;
        qtyInput.GetComponent<TMP_InputField>().text = ingredientListManager.SelectedList.Ingredients[currentIngredient].ToString();

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
        dropdown.value = (int)currentIngredient.Type;
    }

    /// <summary>
    /// Updates the current ingredient with values from inputs
    /// </summary>
    public void OnConfirmClick()
    {
        string qInput = qtyInput.GetComponent<TMP_InputField>().text;
        string qTypeInput = qtyTypeDropdown.GetComponent<TMP_Dropdown>().value.ToString();

        QuantityType parsedQType = currentIngredient.Type; //use the old qtype if parse failed.
        Enum.TryParse(qTypeInput, out parsedQType);

        float parsedQty;
        if (float.TryParse(qInput, out parsedQty))
        {
            //currentIngredient = newIngredient;
            UpdateIngredient(parsedQType, parsedQty);
            ingredientListManager.OpenList(ingredientListManager.SelectedList, this.gameObject);
        }
        else 
        {
            //show some error to user
        }
    }

    private void UpdateIngredient(QuantityType QType, float newQuantity)
    {
        ingredientListManager.SelectedIngredient.Type = QType;
        IngredientList currentList = ingredientListManager.SelectedList;
        currentList.Ingredients[ingredientListManager.SelectedIngredient] = newQuantity;
    }

    /// <summary>
    /// Go back to previous screen
    /// </summary>
    private void OnBackButtonClick()
    {
        ingredientListManager.OpenList(ingredientListManager.SelectedList, this.gameObject);
    }
}
