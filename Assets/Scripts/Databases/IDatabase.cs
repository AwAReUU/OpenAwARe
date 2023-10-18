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

    // returns a dictionary of the material IDs for this ingredient,
    // together with the amount of grams of the material per instance of the ingredient
    public Dictionary<int, float> GetMaterialIDs(Ingredient ingredient);
}

public interface IMaterialDatabase
{
    // returns the full data of a material, given its unique MaterialID
    public ProductMaterial GetMaterial(int id);
}
