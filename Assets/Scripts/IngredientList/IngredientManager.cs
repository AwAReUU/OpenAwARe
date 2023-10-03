using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientManager : MonoBehaviour
{
    [SerializeField] private IngredientListManager ingredientListManager;
    [SerializeField] private GameObject thisScreen;

    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject qtyInput;

    //private Ingredient ingredient;

    void Start() 
    {
        Button backB = backButton.GetComponent<Button>();
        backB.onClick.AddListener(delegate { OnBackButtonClick(); });
    }

    public void OnConfirmClick()
    {
        string searchText = qtyInput.GetComponent<TMP_InputField>().text;
        if (ValidQty(searchText, QuantityType.G))
        {
            //ingredient.SetQuantity(int.Parse(searchText));
            thisScreen.SetActive(false);
            ingredientListManager.OpenList(1);
        }
    }

    private bool ValidQty(string searchText, QuantityType qtyType)
    {
        return true;
        searchText = searchText.Trim();
        //does not need to hold for kg and liter
        if (!searchText.All(char.IsDigit) && qtyType == QuantityType.PCS)
            return false;
        if (searchText.Any(char.IsLetter))
            return false;
        return true;
    }

    public void OnBackButtonClick()
    {
        thisScreen.SetActive(false);
    }
}
