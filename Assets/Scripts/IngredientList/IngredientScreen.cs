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
        currentIngredient = ingredientListManager.CurrentIngredient;

        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        Debug.Log("setting qty");

        currentIngredient = ingredientListManager.CurrentIngredient;
        
        ingredientNameField.GetComponent<TMP_Text>().text = currentIngredient.Name;
        qtyInput.GetComponent<TMP_InputField>().text = ingredientListManager.IngredientLists[ingredientListManager.CurrentListIndex].Ingredients[currentIngredient].ToString();
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
            UpdateIngredient(parsedQty);
            gameObject.SetActive(false);
            ingredientListManager.OpenList(ingredientListManager.CurrentListIndex);
        }
        else 
        {
            //show some error to user
        }
    }

    private void UpdateIngredient(float newQuantity)
    {
        IngredientList currentList = ingredientListManager.IngredientLists[ingredientListManager.CurrentListIndex];
        currentList.Ingredients[ingredientListManager.CurrentIngredient] = newQuantity;
        ingredientListManager.IngredientLists[ingredientListManager.CurrentListIndex] = currentList;
    }

    /// <summary>
    /// Go back to previous screen
    /// </summary>
    private void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        ingredientListManager.OpenList(ingredientListManager.CurrentListIndex);
    }
}
