using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientListScreen : MonoBehaviour
{
    public IngredientListManager ingredientListManager;

    // the objects drawn on screen to display the list
    List<GameObject> ingredientObjects = new List<GameObject>();

    // (assigned within unity)
    public GameObject ingredientObject;
    public TextMeshProUGUI listNameText;
    public GameObject deleteButton;
    public GameObject backButton;
    public GameObject addIngredientButton;

    private void OnEnable()
    {
        DisplayList();
    }

    private void OnDisable()
    {
        RemoveIngredientObjects();
    }

    public void DisplayList()
    {
        RemoveIngredientObjects();

        // display list name
        listNameText.text = ingredientListManager.currentIngredientList.listName;

        float objectDist = 50; // distance between object items
        float yPos;

        // display each ingredient
        for (int i = 0; i < ingredientListManager.currentIngredientList.ingredients.Count; i++)
        {
            // calculate height for ingredient object
            yPos = ingredientObject.transform.localPosition.y - (ingredientObject.GetComponent<RectTransform>().sizeDelta.y + objectDist) * i;

            // create a new list item to display this ingredient
            GameObject newItem = Instantiate(ingredientObject, ingredientObject.transform.parent);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3(ingredientObject.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newItem.SetActive(true);

            // change the text to match the ingredient info
            newItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.currentIngredientList.ingredients[i].name;
            newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.currentIngredientList.ingredients[i].quantity.ToString();
            newItem.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = ingredientListManager.currentIngredientList.ingredients[i].type.ToString();
            int itemIndex = i;
            ingredientObjects.Add(newItem);

            // create a deleteButton for this ingredient
            GameObject newButton = Instantiate(deleteButton, deleteButton.transform.parent);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(deleteButton.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newButton.SetActive(true);
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
            ingredientObjects.Add(newButton);
        }
    }

    /// <summary>
    /// removes currently displayed objects
    /// </summary>
    public void RemoveIngredientObjects()
    {
        foreach (GameObject o in ingredientObjects)
        {
            Destroy(o);
        }
        ingredientObjects = new List<GameObject>();
    }

    public void OnDeleteButtonClick(int i)
    {
        ingredientListManager.DeleteIngredient(i);
        DisplayList();
    }

    public void OnBackButtonClick()
    {
        ingredientListManager.CloseList();
    }

    public void OnAddIngredientButtonClick()
    {
        ingredientListManager.AddIngredient(new Ingredient("banana", QuantityType.PCS, 3));
        DisplayList();
    }
}
