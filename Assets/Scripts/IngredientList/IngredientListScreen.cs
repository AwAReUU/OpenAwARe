using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientListScreen : MonoBehaviour
{
    [SerializeField] private IngredientListManager ingredientListManager;

    // the objects drawn on screen to display the list
    List<GameObject> ingredientObjects = new List<GameObject>();
    GameObject addIngredientObj;

    // (assigned within unity)
    [SerializeField] private GameObject thisScreen;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject listItemObject;
    [SerializeField] private TextMeshProUGUI listNameText;
    [SerializeField] private GameObject addIngredientButton;
    [SerializeField] private Transform scrollViewContent;

    private void OnEnable()
    {
        backButton.SetActive(true);
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

        // display each ingredient
        for (int i = 0; i < ingredientListManager.currentIngredientList.ingredients.Count; i++)
        {
            // create a new list item to display this ingredient
            GameObject listItem = Instantiate(listItemObject, scrollViewContent);
            listItem.SetActive(true);

            // change the text to match the ingredient info
            Button ingredientButton = listItem.transform.GetChild(0).GetComponent<Button>();
            Button delButton = listItem.transform.GetChild(1).GetComponent<Button>();

            List<Ingredient> ingredients = ingredientListManager.currentIngredientList.ingredients;
            ingredientButton.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].name;
            ingredientButton.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].quantity.ToString();
            ingredientButton.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].type.ToString();
            ingredientObjects.Add(listItem);

            // create a deleteButton for this ingredient
            int itemIndex = i;
            ingredientButton.onClick.AddListener(() => { OnIngredientButtonClick(itemIndex); });
            delButton.onClick.AddListener(() => { OnDeleteButtonClick(itemIndex); });
        }
        //Add the "Add ingredient" button to the bottom of the list.
        addIngredientObj = Instantiate(addIngredientButton, scrollViewContent);
        Button addIngredientBtn = addIngredientButton.GetComponent<Button>();
        addIngredientBtn.onClick.AddListener(() => { OnAddIngredientButtonClick(); });
        addIngredientObj.SetActive(true);
    }

    private void OnIngredientButtonClick(int itemIndex)
    {
        Ingredient ingredient = ingredientListManager.currentIngredientList.ingredients[itemIndex];
        thisScreen.SetActive(false);
        ingredientListManager.OpenIngredientScreen(ingredient);
    }

    /// <summary>
    /// removes all objects from the scrollview
    /// </summary>
    public void RemoveIngredientObjects()
    {
        foreach (GameObject o in ingredientObjects)
        {
            Destroy(o);
        }
        Destroy(addIngredientObj);
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
        backButton.SetActive(false);
    }

    public void OnAddIngredientButtonClick()
    {
        ingredientListManager.OpenSearchScreen();
        //ingredientListManager.AddIngredient(new Ingredient("banana", QuantityType.PCS, 3));
        //DisplayList();
    }
}
