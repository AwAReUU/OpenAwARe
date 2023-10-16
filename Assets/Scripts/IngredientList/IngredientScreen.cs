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
    [SerializeField] private GameObject thisScreen;

    [SerializeField] private GameObject ingredientNameField;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject qtyInput;
    [SerializeField] private GameObject qtyTypeDropdown;

    private Ingredient currentIngredient;

    void OnEnable()
    {
        currentIngredient = ingredientListManager.ingredientLists[ingredientListManager.currentListIndex].ingredients[ingredientListManager.currentIngredientIndex];

        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        SetDropDownItems();
        
        ingredientNameField.GetComponent<TMP_Text>().text = currentIngredient.name;
        qtyInput.GetComponent<TMP_InputField>().text = currentIngredient.quantity.ToString();
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
        dropdown.value = (int)currentIngredient.type;
    }

    /// <summary>
    /// Updates the current ingredient with values from inputs
    /// </summary>
    public void OnConfirmClick()
    {
        string qInput = qtyInput.GetComponent<TMP_InputField>().text;
        string qTypeInput = qtyTypeDropdown.GetComponent<TMP_Dropdown>().value.ToString();

        QuantityType parsedQType = currentIngredient.type; //use the old qtype if parse failed.
        Enum.TryParse(qTypeInput, out parsedQType);

        float parsedQty;
        if (float.TryParse(qInput, out parsedQty))
        {
            Ingredient newIngredient = new Ingredient
            (
                currentIngredient.ID,
                currentIngredient.name,
                parsedQType,
                parsedQty
            );
            //currentIngredient = newIngredient;
            UpdateIngredient(newIngredient);
            thisScreen.SetActive(false);
            ingredientListManager.OpenList(ingredientListManager.currentListIndex);
        }
        else 
        {
            //show some error to user
        }
    }

    /// <summary>
    /// Overwrite the ingredient in the current ingredient list with
    /// the newly instantiated ingredient.
    /// </summary>
    /// <param name="newIngredient"></param>
    private void UpdateIngredient(Ingredient newIngredient)
    {
        IngredientList currentList = ingredientListManager.ingredientLists[ingredientListManager.currentListIndex];
        currentList.ingredients[ingredientListManager.currentIngredientIndex] = newIngredient;
        ingredientListManager.ingredientLists[ingredientListManager.currentListIndex] = currentList;
    }

    /// <summary>
    /// Go back to previous screen
    /// </summary>
    private void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
        ingredientListManager.OpenList(ingredientListManager.currentListIndex);
    }
}
