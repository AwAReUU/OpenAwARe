using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInfoScreen : MonoBehaviour
{
    [SerializeField] private IngredientListManager ingredientListManager;

    [SerializeField] private Ingredient ingredient;
    [SerializeField] private GameObject backButton;

    private void OnEnable()
    {
        // TODO: draw objects
    }

    private void OnDisable()
    {
        // TODO: remove drawn objects
    }
}
