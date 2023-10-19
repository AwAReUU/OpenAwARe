using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MockupIngredientDatabase : IIngredientDatabase
{
    readonly Dictionary<int, Ingredient> database; // (ID , Ingredient)

    public MockupIngredientDatabase()
    {
        database = new()
        {
            { 0,  new Ingredient(0,"Sugar"      , QuantityType.G  , new Dictionary<int, float> { {4,0.5f}                })},
            { 1,  new Ingredient(1,"Banana"     , QuantityType.PCS, new Dictionary<int, float> { {4,1},{6,3}             })},
            { 2,  new Ingredient(2,"Strawberry" , QuantityType.G  , new Dictionary<int, float> { {5,1},{6,0.2f}          })},
            { 3,  new Ingredient(3,"Beef"       , QuantityType.G  , new Dictionary<int, float> { {2,0.5f},{6,5}          })},
            { 4,  new Ingredient(4,"Pork"       , QuantityType.G  , new Dictionary<int, float> { {2,1},{3,2}             })},
            { 5,  new Ingredient(5,"Chicken"    , QuantityType.G  , new Dictionary<int, float> { {1,1},{6,0.2f}          })},
            { 6,  new Ingredient(6,"Water"      , QuantityType.L  , new Dictionary<int, float> { {6,1}                   })},
            { 7,  new Ingredient(7,"Milk"       , QuantityType.L  , new Dictionary<int, float> { {0,2},{2,0.3f},{5,0.4f} })},
            { 8,  new Ingredient(8,"Kiwi Fruit" , QuantityType.PCS, new Dictionary<int, float> { {1,4},{3,3},{4,0.5f}    })},
            { 9,  new Ingredient(9,"Pineapple"  , QuantityType.G  , new Dictionary<int, float> { {2,0.5f},{3,3},{5,0.4f} })},
            { 10, new Ingredient(10,"Melon"     , QuantityType.G  , new Dictionary<int, float> { {2,4},{6,2}             })},
            { 11, new Ingredient(11,"Pear"      , QuantityType.PCS, new Dictionary<int, float> { {0,1},{2,4},{3,3}       })},
            { 12, new Ingredient(12,"Mandarin"  , QuantityType.PCS, new Dictionary<int, float> { {0,2},{3,3},{4,1.5f}    })},
            { 13, new Ingredient(13,"Orange"    , QuantityType.PCS, new Dictionary<int, float> { {1,2},{5,1}             })},
            { 14, new Ingredient(14,"Grape"     , QuantityType.G  , new Dictionary<int, float> { {0,2}                   })},
            { 15, new Ingredient(15,"Apple"     , QuantityType.PCS, new Dictionary<int, float> { {3,2},{5,3},{6,0.5f}    })}

        };
    }

    private readonly List<(int, string)> SearchTable = new()
    {
        (0, "Sugar"),
        (1, "Banana"),
        (2, "Strawberry"),
        (3, "Beef"), (3, "Steak"), (3, "Hamburger"),
        (4, "Pork"), (4, "Bacon"), (4, "Ham"),
        (5, "Chicken"), (5, "Chicken Legs"), (5, "Chicken Wings"), (5, "Drumsticks"),
        (6, "Water"),
        (7, "Milk"),
        (8, "Kiwi Fruit"),
        (9, "Pineapple"), (9, "Ananas"),
        (10, "Melon"), (10, "Watermelon"),
        (11, "Pear"),
        (12, "Mandarin"), (12, "Satsuma"),
        (13, "Orange"), (13, "Tangerine"),
        (14, "Grape"), (14, "Red Grape"), (14, "White Grape"),
        (15, "Apple"), (15, "Red Apple"), (15, "Green Apple"), (15, "Fuji Apple"), (15, "Elstar Apple"), (15, "Pink Lady"),
    };

    public List<Ingredient> Search(string term)
    {
        List<int> ids = SearchTable.Where(x => x.Item2.Contains(term, System.StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).ToList();
        List<Ingredient> result = database.Where(x => ids.Contains(x.Key)).Select(x => x.Value).ToList();
        //List<Ingredient> result = GetIngredients(ids);
        return result;
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

    public Dictionary<int, float> GetResourceIDs(Ingredient ingredient)
    {
        return ingredient.Resources;
    }
}
