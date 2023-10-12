using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IDatabase
{
    // returns a List of Ingredients with a (possible) name containing the search term
    public List<Ingredient> Search(string term);

    // returns the full data of an ingredient, given its unique IngredientID
    public Ingredient getIngredient(int ingredientID);
}

public class DatabaseMockup : IDatabase
{
    private static (int, string)[] SearchTableArray =
    {
        ( 1, "Apple"), (1, "Red Apple"), (1, "Green Apple"), (1, "Fuji Apple"), (1, "Elstar Apple"), (1, "Pink Lady"),
        ( 2, "Banana"),
        ( 3, "Pear"),
        ( 4, "Mandarin"), ( 4, "Satsuma"),
        ( 5, "Orange"), ( 5, "Tangerine"),
        ( 6, "Grape"), (6, "Red Grape"), (6, "White Grape"),
        ( 7, "Strawberry"),
        ( 8, "Kiwi Fruit"),
        ( 9, "Pineapple"), (9, "Ananas"),
        (10, "Melon"), (10, "Watermelon"),
        (11, "Beef"), (11, "Steak"), (11, "Hamburger"),
        (12, "Chicken"), (12, "Chicken Legs"), (12, "Chicken Wings"), (12, "Drumsticks"),
        (13, "Pork"), (13, "Bacon"), (13, "Ham"),
        (14, "Duck"),
        (15, "Milk")
    };
    private List<(int, string)> SearchTable = new List<(int, string)>(SearchTableArray);

    private static (int, string, string, QuantityType, int)[] IngredientTableArray =
    {
        (1,      "Apple",  "fruit", QuantityType.G, 100),
        (2,     "Banana",  "fruit", QuantityType.G, 100),
        (3,       "Pear",  "fruit", QuantityType.G, 100),
        (4,   "Mandarin",  "fruit", QuantityType.G, 100),
        (5,     "Orange",  "fruit", QuantityType.G, 100),
        (6,      "Grape",  "fruit", QuantityType.G, 100),
        (7, "Strawberry",  "fruit", QuantityType.G, 100),
        (8, "Kiwi Fruit",  "fruit", QuantityType.G, 100),
        (9,  "Pineapple",  "fruit", QuantityType.G, 100),
        (10,     "Melon",  "fruit", QuantityType.G, 100),
        (11,      "Beef", "animal", QuantityType.G, 100),
        (12,   "Chicken", "animal", QuantityType.G, 100),
        (13,      "Pork", "animal", QuantityType.G, 100),
        (14,      "Duck", "animal", QuantityType.G, 100),
        (15,      "Milk", "animal", QuantityType.G, 100)
    };
    private List<(int, string, string, QuantityType, int)> IngredientTable = 
           new List<(int, string, string, QuantityType, int)>(IngredientTableArray);

    List<Ingredient> IDatabase.Search(string term)
    {
        List<int> ids = SearchTable.Where(x => x.Item2.Contains(term,System.StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).ToList();
        List<Ingredient> result = IngredientTable.Where(x => ids.Contains(x.Item1)).Select(x => new Ingredient(x.Item1,x.Item2,x.Item4,0)).ToList();
        return result;
    }

    Ingredient IDatabase.getIngredient(int ingredientID)
    {
        return new Ingredient(0, "Test", QuantityType.G, 0);
    }
}