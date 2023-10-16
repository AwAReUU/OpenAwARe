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
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        SetDropDownItems();

        currentIngredient = ingredientListManager.ingredientLists[ingredientListManager.currentListIndex].ingredients[ingredientListManager.currentIngredientIndex];
        
        ingredientNameField.GetComponent<TMP_Text>().text = currentIngredient.name;
        qtyInput.GetComponent<TMP_InputField>().text = currentIngredient.quantity.ToString();
    }

    private void SetDropDownItems()
    {
        //convert QuantityType names to string list
        List<string> m_DropOptions = Enum.GetNames(typeof(QuantityType)).ToList();

        //Fetch the Dropdown GameObject the script is attached to
        TMP_Dropdown d = qtyTypeDropdown.GetComponent<TMP_Dropdown>();
        d.ClearOptions();
        d.AddOptions(m_DropOptions);
    }

    public void OnConfirmClick()
    {
        string input = qtyInput.GetComponent<TMP_InputField>().text;

        float parsedInput;
        if (float.TryParse(input, out parsedInput))
        {
            Ingredient newIngredient = new Ingredient
            (
                currentIngredient.ID,
                currentIngredient.name,
                currentIngredient.type,
                parsedInput
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

    private void UpdateIngredient(Ingredient newIngredient)
    {
        IngredientList currentList = ingredientListManager.ingredientLists[ingredientListManager.currentListIndex];
        currentList.ingredients[ingredientListManager.currentIngredientIndex] = newIngredient;
        ingredientListManager.ingredientLists[ingredientListManager.currentListIndex] = currentList;
    }

    private void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
        ingredientListManager.OpenList(ingredientListManager.currentListIndex);
    }
}
