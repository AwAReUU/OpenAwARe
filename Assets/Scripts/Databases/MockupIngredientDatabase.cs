using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockupIngredientDatabase : IIngredientDatabase
{
    Dictionary<int, Ingredient> database; // (ID , Ingredient)

    public MockupIngredientDatabase()
    {
        database = new()
        {
            { 0, new Ingredient(0,"banana"    , QuantityType.PCS, new Dictionary<int, float> { {4,1},{6,3}          })},
            { 1, new Ingredient(1,"strawberry", QuantityType.G  , new Dictionary<int, float> { {5,1},{6,0.2f}          })},
            { 2, new Ingredient(2,"cow"       , QuantityType.G  , new Dictionary<int, float> { {2,0.5f},{6,5}       })},
            { 3, new Ingredient(3,"pig"       , QuantityType.G  , new Dictionary<int, float> { {2,1},{3,2}          })},
            { 4, new Ingredient(4,"chicken"   , QuantityType.G  , new Dictionary<int, float> { {1,1},{6,0.2f}       })},
            { 5, new Ingredient(5,"water"     , QuantityType.L  , new Dictionary<int, float> { {6,1}                })},
            { 6, new Ingredient(6,"milk"      , QuantityType.L  , new Dictionary<int, float> { {0,2},{3,3},{5,0.4f} })}
        };
    }

    public List<Ingredient> Search(string s)
    {
        //TODO
        return null;
    }

    public Ingredient GetIngredient(int id)
    {
        return database[id];
    }

    public List<Ingredient> GetIngredients(List<int> ids)
    {
        List<Ingredient> ingredients = new();
        foreach (int id in ids)
        {
            ingredients.Add(GetIngredient(id));
        }
        return ingredients;
    }

    public Dictionary<int, float> GetMaterialIDs(Ingredient ingredient)
    {
        return ingredient.Materials;
    }
}
