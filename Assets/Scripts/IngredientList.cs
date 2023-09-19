using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientList
{
    public List<Ingredient> ingredients { get; private set; }

    public string listName { get; private set; }

    public IngredientList(string listName, List<Ingredient> ingredients = null)
    {
        this.listName = listName;

        if (ingredients != null)
            this.ingredients = ingredients;
        else
            this.ingredients = new List<Ingredient>();
    }

    public int NumberOfIngredients()
    {
        return ingredients.Count;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
    }

    public void RemoveIngredient(int i)
    {
        ingredients.Remove(ingredients[i]);
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