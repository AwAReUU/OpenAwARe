// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwARe.IngredientList.Logic;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of the Ingredient Database interface, that uses mock database using a locally saved table.
    /// The Mock Database can be used for testing purposes.
    /// </summary>
    public class MockupIngredientDatabase : IIngredientDatabase
    {
        /// <summary>
        /// List in which mock ingredients are stored.
        /// </summary>
        readonly List<Ingredient> ingredientTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockupIngredientDatabase"/> class.
        /// </summary>
        public MockupIngredientDatabase()
        {
            ingredientTable = new()
            {
                new Ingredient( 1,           "Water",  1.0f, null),
                new Ingredient( 2,           "Apple",  null,  100),
                new Ingredient( 3,          "Banana",  null,  200),
                new Ingredient( 4,            "Pear",  null,  150),
                new Ingredient( 5,        "Mandarin",  null,   60),
                new Ingredient( 6,          "Orange",  null,  100),
                new Ingredient( 7,           "Grape",  null,    8),
                new Ingredient( 8,      "Strawberry",  null,    7),
                new Ingredient( 9,      "Kiwi Fruit",  null,   60),
                new Ingredient(10,       "Pineapple",  null, 1000),
                new Ingredient(11,           "Melon",  null, 1000),
                new Ingredient(12,            "Beef",  null,  250),
                new Ingredient(13,         "Chicken",  null,  250),
                new Ingredient(14,            "Pork",  null,  250),
                new Ingredient(15,            "Duck",  null,  250),
                new Ingredient(16,        "Cow Milk", 1.04f, null),
                new Ingredient(17,          "Potato",  null,  100),
                new Ingredient(18,            "Beet",  null,  150),
                new Ingredient(19,       "Artichoke",  null,  120),
                new Ingredient(20,        "Broccoli",  null,  300),
                new Ingredient(21,         "Cabbage",  null, 1000),
                new Ingredient(22,          "Carrot",  null,   60),
                new Ingredient(23,            "Corn",  null,  150),
                new Ingredient(24,        "Cucumber",  null,  300),
                new Ingredient(25,        "Eggplant",  null,  400),
                new Ingredient(26,          "Garlic",  null,    5),
                new Ingredient(27,           "Onion",  null,  100),
                new Ingredient(28,          "Pepper",  null,   45),
                new Ingredient(29,     "Poppy Seeds",  null,    4),
                new Ingredient(30,         "Pumpkin",  null, 2000),
                new Ingredient(31, "Sunflower Seeds",  null,    4),
                new Ingredient(32,          "Tomato",  null,  100),
                new Ingredient(33,           "Wheat",  null,   45),
                new Ingredient(34,           "Pasta",  null, null),
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
            (16,      "Cow Milk"), (16,          "Milk"),
            (17, "Potato"),
            (18, "Beet"),
            (19, "Artichoke"),
            (20, "Broccoli"),
            (21, "Cabbage"),
            (22, "Carrot"),
            (23, "Corn"),
            (24, "Cucumber"),
            (25, "Eggplant"),
            (26, "Garlic"),
            (27, "Onion"),
            (28, "Pepper"),
            (29, "Poppy"), (29, "Poppy Seeds"),
            (30, "Pumpkin"),
            (31, "Sunflower"), (33, "Sunflower Seeds"),
            (32, "Tomato"),
            (33, "Wheat"),
            (34, "Pasta"), (34, "Macaroni"), (34, "Spaghetti"), (34, "Penne")
        };

        ///<inheritdoc cref="IIngredientDatabase.Search"/>
        public Task<Ingredient[]> Search(string term)
        {
            IEnumerable<int> ids = (
                from (int ID, string Name) s in SearchTable
                    // find the index of the search term in the possible name of the ingredient
                let index = s.Name.IndexOf(term, System.StringComparison.OrdinalIgnoreCase)
                // IndexOf returns -1 if not found, filter these out
                where index > -1
                // order by the index of the search term, then by name
                orderby (index, s.Name)
                select s.ID).Distinct();


            // get the ingredients with these found IDs
            return Task.Run(() => GetIngredients(ids));
        }

        ///<inheritdoc cref="IIngredientDatabase.GetIngredient"/>
        public Task<Ingredient> GetIngredient(int id)
        {
            // return the first ingredient that matches id, should be the only one since IDs are unique
            return Task.Run(() => ingredientTable.First(x => x.IngredientID == id));
        }

        ///<inheritdoc cref="IIngredientDatabase.GetIngredients"/>
        public Task<Ingredient[]> GetIngredients(IEnumerable<int> ids)
        {
            // perform an inner join of ingredient table and ids on ingredientID
            IEnumerable<Ingredient> ingredients =
                from id in ids
                join ingredient in ingredientTable on id equals ingredient.IngredientID
                select ingredient;
            return Task.Run(() => ingredients.ToArray());
        }
    }
}
