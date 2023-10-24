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

public interface IResourceDatabase
    {
        // returns the full data of a resource, given its unique Resource ID
        public Resource GetResource(int id);
        public List<Resource> GetResources(List<int> ids);
    }
