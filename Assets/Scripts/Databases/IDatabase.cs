using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIngredientDatabase
{
    // returns a List of Ingredients with a (possible) name containing the search term
    public List<Ingredient> Search(string term);

    // returns the full data of an ingredient, given its unique IngredientID
    public Ingredient GetIngredient(int id);
    public List<Ingredient> GetIngredients(List<int> ids);

    public Dictionary<int, float> GetResourceIDs(Ingredient ingredient);
}

public interface IIngredientToResourceDatabase
{
    // returns a dictionary of the resource IDs for this ingredient,
    // together with the amount of grams of the resource per instance of the ingredient
    public Dictionary<int, float> GetResourceIDs(Ingredient ingredient);
}

public interface IResourceDatabase

{
    // returns the full data of a resource, given its unique Resource ID
    public Resource GetResource(int id);
}
