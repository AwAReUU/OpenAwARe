using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientList : ScriptableObject
{
    public List<Ingredient> ingredients;
    List<GameObject> ingredientObjects = new List<GameObject>();

    public string listName;

    GameObject ingredientObject;
    GameObject deleteButton;

    public IngredientList(string listName, GameObject ingredientObject, GameObject deleteButton, List<Ingredient> ingredients = null)
    {
        this.listName = listName;

        this.ingredientObject = ingredientObject;
        this.deleteButton = deleteButton;

        if (ingredients != null)
            this.ingredients = ingredients;
        else
            this.ingredients = new List<Ingredient>();

        
        this.deleteButton = deleteButton;
    }

    /// <summary>
    /// the number of ingredients in the list
    /// </summary>
    /// <returns></returns>
    public int NumberOfIngredients()
    {
        return ingredients.Count;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
        DisplayList();
    }

    public void RemoveIngredient(int i)
    {
        ingredients.Remove(ingredients[i]);
        DisplayList();
    }

    public void DisplayList()
    {
        RemoveIngredientObjects();

        float objectDist = 50; // distance between object items
        float yPos;

        // display the name of the ingredient list
        GameObject textObject = new GameObject("ListName");
        textObject.transform.SetParent(ingredientObject.transform.parent);
        TMP_Text name = textObject.AddComponent<TextMeshProUGUI>();
        name.text = listName;
        name.fontSize = 80;
        name.transform.localPosition = new Vector3(0, ingredientObject.transform.localPosition.y + ingredientObject.GetComponent<RectTransform>().sizeDelta.y + objectDist, 0);
        name.GetComponent<RectTransform>().sizeDelta = new Vector3(500, 100, 0);
        //name.color = Color.black;
        name.alignment = TextAlignmentOptions.Center;
        ingredientObjects.Add(textObject);

        // display each ingredient
        for (int i = 0; i < ingredients.Count; i++)
        {
            // calculate height for ingredient object
            yPos = ingredientObject.transform.localPosition.y - (ingredientObject.GetComponent<RectTransform>().sizeDelta.y + objectDist) * i;

            // create a new list item to display this ingredient
            GameObject newItem = Instantiate(ingredientObject, ingredientObject.transform.parent);
            newItem.GetComponent<RectTransform>().localPosition = new Vector3(ingredientObject.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newItem.SetActive(true);

            // change the text to match the ingredient info
            newItem.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].name;
            newItem.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].quantity.ToString();
            newItem.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = ingredients[i].type.ToString();
            int itemIndex = i;
            ingredientObjects.Add(newItem);

            // create a deleteButton for this ingredient
            GameObject newButton = Instantiate(deleteButton, deleteButton.transform.parent);
            newButton.GetComponent<RectTransform>().localPosition = new Vector3(deleteButton.transform.localPosition.x, yPos, 0); // set item position to the correct height
            newButton.SetActive(true);
            newButton.transform.GetComponent<Button>().onClick.AddListener(() => { RemoveIngredient(itemIndex); });
            ingredientObjects.Add(newButton);
        }
    }

    /// <summary>
    /// remove currently displayed objects
    /// </summary>
    public void RemoveIngredientObjects()
    {
        foreach (GameObject o in ingredientObjects)
        {
            Destroy(o);
        }
        ingredientObjects = new List<GameObject>();
    }
}

public class Ingredient
{
    public string name { get; private set; }
    public QuantityType type { get; private set; }
    public float quantity { get; private set; }

    public Ingredient(string name, QuantityType type, float quantity)
    {
        this.name = name;
        this.type = type;
        this.quantity = quantity;
    }

    void SetQuantity(float q)
    {
        quantity = q;
    }
}

/*
public struct IngredientStruct
{
    public string name { get; private set; }
    public QuantityType type { get; private set; }
    public float quantity { get; private set; }

    public IngredientStruct(string name, QuantityType type, float quantity)
    {
        this.name = name;
        this.type = type;
        this.quantity = quantity;
    }
}
*/
public enum QuantityType
{ 
    G,
    PCS,
    L
}