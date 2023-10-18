using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientList
{
    public Dictionary<Ingredient, float> Ingredients { get; private set; }

    public string ListName { get; private set; }

    public IngredientList(string listName, Dictionary<Ingredient, float> ingredients = null)
    {
        this.ListName = listName;

        if (ingredients != null)
            this.Ingredients = ingredients;
        else
            this.Ingredients = new();
    }

    public int NumberOfIngredients()
    {
        return Ingredients.Count;
    }

    public void AddIngredient(Ingredient ingredient, float quantity)
    {
        Ingredients.Add(ingredient, quantity);
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        Ingredients.Remove(ingredient);
    }

    
}

public class Ingredient : IEquatable<Ingredient>
{
    public int ID { get; }
    public string Name { get; private set; }
    public QuantityType Type { get; set; }

    public Dictionary<int, float> Materials { get; private set; }

    public Ingredient(int id, string name, QuantityType type, Dictionary<int, float> materials)
    {
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Materials = materials;
    }
    public override bool Equals(object obj) => obj is Ingredient m && this.Equals(m);

    public bool Equals(Ingredient m) => ID == m.ID;

    public override int GetHashCode() => ID.GetHashCode();

}

public enum QuantityType
{ 
    G,   // grams
    PCS, // pieces
    L    // litres
}

