using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddIngredientScreen : MonoBehaviour
{
    [SerializeField] private MainManager mainManager;

    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject addButton;

    Ingredient ingredient;

    private void OnEnable()
    {
        // TODO: draw objects
    }

    private void OnDisable()
    {
        // TODO: remove drawn objects
    }

    public void OnBackButtonClick()
    {
        // TODO: go back to IngredientListScreen
    }

    public void OnAddButtonClick()
    {
        mainManager.AddIngredient(ingredient);
    }
}
