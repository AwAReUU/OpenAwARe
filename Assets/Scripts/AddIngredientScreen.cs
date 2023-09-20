using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIngredientScreen : MonoBehaviour
{
    public IngredientListManager ingredientListManager;

    public GameObject backButton;
    public GameObject addButton;

    Ingredient ingredient;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void OnBackButtonClick()
    {
        
    }

    public void OnAddButtonClick()
    {
        ingredientListManager.AddIngredient(ingredient);
    }
}
