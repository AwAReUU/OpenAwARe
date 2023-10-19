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

    private Ingredient currentIngredient;

    void OnEnable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        Debug.Log("setting qty");

        currentIngredient = ingredientListManager.CurrentIngredient;
        
        ingredientNameField.GetComponent<TMP_Text>().text = currentIngredient.Name;
        qtyInput.GetComponent<TMP_InputField>().text = ingredientListManager.IngredientLists[ingredientListManager.CurrentListIndex].Ingredients[currentIngredient].ToString();
    }

    public void OnConfirmClick()
    {
        string input = qtyInput.GetComponent<TMP_InputField>().text;

        float parsedInput;
        if (float.TryParse(input, out parsedInput))
        {
            //currentIngredient = newIngredient;
            UpdateIngredient(parsedInput);
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

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        ingredientListManager.OpenList(ingredientListManager.CurrentListIndex);
    }
}
