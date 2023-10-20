using System.Collections.Generic;
using System.Linq;
using IngredientLists;

namespace Databases
{
    public class MockupIngredientDatabase : IIngredientDatabase
    {
        readonly List<Ingredient> ingredientTable; // (ID , Ingredient)

        public MockupIngredientDatabase()
        {
            ingredientTable = new()
            {
                new Ingredient( 1,     "Water",  1.0f, null),
                new Ingredient( 2,     "Apple",  null,  100),
                new Ingredient( 3,    "Banana",  null,  200),
                new Ingredient( 4,      "Pear",  null,  150),
                new Ingredient( 5,  "Mandarin",  null,   60),
                new Ingredient( 6,    "Orange",  null,  100),
                new Ingredient( 7,     "Grape",  null,    8),
                new Ingredient( 8,"Strawberry",  null,    7),
                new Ingredient( 9,"Kiwi Fruit",  null,   60),
                new Ingredient(10, "Pineapple",  null, 1000),
                new Ingredient(11,     "Melon",  null, 1000),
                new Ingredient(12,      "Beef",  null,  250),
                new Ingredient(13,   "Chicken",  null,  250),
                new Ingredient(14,      "Pork",  null,  250),
                new Ingredient(15,      "Duck",  null,  250),
                new Ingredient(16,      "Milk", 1.04f, null)
            };
        }

        private readonly List<(int, string)> SearchTable = new()
        {
            ( 1,         "Water"),
            ( 2,         "Apple"), ( 2,     "Red Apple"), ( 2,   "Green Apple"), ( 2,    "Fuji Apple"), ( 2,  "Elstar Apple"), ( 2,     "Pink Lady"),
            ( 3,        "Banana"),
            ( 4,          "Pear"),
            ( 5,      "Mandarin"), ( 5,       "Satsuma"),
            ( 6,        "Orange"), ( 6,     "Tangerine"),
            ( 7,         "Grape"), ( 7,     "Red Grape"), ( 7,   "White Grape"),
            ( 8,    "Strawberry"),
            ( 9,    "Kiwi Fruit"),
            (10,     "Pineapple"), (10,        "Ananas"),
            (11,         "Melon"), (11,    "Watermelon"),
            (12,          "Beef"), (12,         "Steak"), (12,     "Hamburger"),
            (13,       "Chicken"), (13,  "Chicken Legs"), (13, "Chicken Wings"), (13,    "Drumsticks"),
            (14,          "Pork"), (14,         "Bacon"), (14,           "Ham"),
            (15,          "Duck"),
            (16,          "Milk"),
        };

        public List<Ingredient> Search(string term)
        {
            List<int> ids = SearchTable.Where(x => x.Item2.Contains(term, System.StringComparison.OrdinalIgnoreCase)).Select(x => x.Item1).ToList();
            List<Ingredient> result = ingredientTable.Where(x => ids.Contains(x.ID)).ToList();
            //List<Ingredient> result = GetIngredients(ids);
            return result;
        }

        public Ingredient GetIngredient(int id)
        {
            return ingredientTable.First(x => x.ID == id);
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
