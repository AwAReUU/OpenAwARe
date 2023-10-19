using System.Collections.Generic;
using System.Linq;
using IngredientLists;

namespace Databases
{
    public class MockupIngredientDatabase : IIngredientDatabase
    {
        readonly Dictionary<int, Ingredient> database; // (ID , Ingredient)

        public MockupIngredientDatabase()
        {
            database = new()
        {
            { 0,  new Ingredient(0,"Sugar"      ,null , null)},
            { 1,  new Ingredient(1,"Banana"     ,null , 200 )},
            { 2,  new Ingredient(2,"Strawberry" ,null , 50  )},
            { 3,  new Ingredient(3,"Beef"       ,null , null)},
            { 4,  new Ingredient(4,"Pork"       ,null , null)},
            { 5,  new Ingredient(5,"Chicken"    ,null , 300 )},
            { 6,  new Ingredient(6,"Water"      ,1    , null)},
            { 7,  new Ingredient(7,"Milk"       ,1.03f, null)},
            { 8,  new Ingredient(8,"Kiwi Fruit" ,null , 200 )},
            { 9,  new Ingredient(9,"Pineapple"  ,null , 300 )},
            { 10, new Ingredient(10,"Melon"     ,null , 1000)},
            { 11, new Ingredient(11,"Pear"      ,null , 500 )},
            { 12, new Ingredient(12,"Mandarin"  ,null , 300 )},
            { 13, new Ingredient(13,"Orange"    ,null , 350 )},
            { 14, new Ingredient(14,"Grape"     ,null , 20  )},
            { 15, new Ingredient(15,"Apple"     ,null , 500 )}

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
    }
}
