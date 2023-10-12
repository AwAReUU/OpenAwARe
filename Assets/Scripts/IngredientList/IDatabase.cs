using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIngredientDatabase
{
    public List<Ingredient> Search(string s);
    public Ingredient GetIngredient(float id);
    public Dictionary<ProductMaterial, float> GetMaterials(Ingredient ingredient);
}

public class MockupIngredientDatabase : IIngredientDatabase
{
    Dictionary<float, Ingredient> database;

    MockupIngredientDatabase()
    {
        database = new();
    }

    public List<Ingredient> Search(string s)
    {
        return new();
    }

    public Ingredient GetIngredient(float id)
    {
        return null;
    }

    public Dictionary<ProductMaterial, float> GetMaterials(Ingredient ingredient)
    {
        Dictionary<ProductMaterial, float> materials = new();
        //TODO: get materials from database
        materials.Add(new ProductMaterial(1, MaterialType.Plant),1);
        materials.Add(new ProductMaterial(3, MaterialType.Water), 0.8f);
        materials.Add(new ProductMaterial(6, MaterialType.Animal), 0.2f);
        return materials;
    }
}