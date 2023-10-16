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

public class MockupIngredientDatabase : IIngredientDatabase
{
    Dictionary<int, Ingredient> database;

    public MockupIngredientDatabase()
    {
        database = new();
    }

    public List<Ingredient> Search(string s)
    {
        return new();
    }

    public Ingredient GetIngredient(int id)
    {
        return database[id];
    }

    public List<Ingredient> GetIngredients(List<int> ids)
    {
        return null;
    }

    public Dictionary<int, float> GetMaterialIDs(Ingredient ingredient)
    {
        Dictionary<int, float> materialIDs = new();
        //TODO: get materials from database
        materialIDs.Add(1,1);
        materialIDs.Add(3, 0.8f);
        materialIDs.Add(6, 0.2f);
        return materialIDs;
    }
}

public interface IMaterialDatabase
{
    public ProductMaterial GetMaterial(int id);
}

public class MockupMaterialDatabase : IMaterialDatabase
{
    Dictionary<int, ProductMaterial> database;

    public MockupMaterialDatabase()
    {
        database = new();
    }

    public ProductMaterial GetMaterial(int id)
    {
        return database[id];
    }
}