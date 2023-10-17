using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIngredientDatabase
{
    public List<Ingredient> Search(string s);
    public Ingredient GetIngredient(int id);
    public List<Ingredient> GetIngredients(List<int> ids);
    public Dictionary<int, float> GetMaterialIDs(Ingredient ingredient);
}

public interface IMaterialDatabase
{
    public ProductMaterial GetMaterial(int id);
}
