using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientManager : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;
    [SerializeField] private GameObject thisScreen;

    [SerializeField] private GameObject ingredientNameField;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject qtyInput;

    void OnEnable()
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });

        Debug.Log("setting qty");
        ingredientNameField.GetComponent<TMP_Text>().text = mainManager.currentIngredient.name;
        qtyInput.GetComponent<TMP_InputField>().text = mainManager.currentIngredient.quantity.ToString();
    }

    public void OnConfirmClick()
    {
        string input = qtyInput.GetComponent<TMP_InputField>().text;

        float parsedInput;
        if (float.TryParse(input, out parsedInput))
        {
            Ingredient newIngredient = new Ingredient
            (
                mainManager.currentIngredient.name,
                mainManager.currentIngredient.type,
                parsedInput
            );
            mainManager.currentIngredient = newIngredient;
            thisScreen.SetActive(false);
            mainManager.OpenList(mainManager.currentListIndex);
        }
        else 
        {
            //show some error to user
        }

    }

    public void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
        mainManager.OpenList(mainManager.currentListIndex);
    }
}
