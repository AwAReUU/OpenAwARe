// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;

using AwARe.IngredientList.Logic;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of the Ingredient-to-Resource Database interface, that uses mock database using a locally saved table.
    /// The Mock Database can be used for testing purposes.
    /// </summary>
    public class MockupIngrToResDatabase : IIngrToResDatabase
    {
        private readonly List<(int, int, float)> requiresTable; // (IngrID, ResID, ResGrams per IngrGram)

        /// <summary>
        /// Initializes a new instance of the <see cref="MockupIngrToResDatabase"/> class.
        /// </summary>
        public MockupIngrToResDatabase()
        {
            requiresTable = new()
            {
                ( 1,  1,    1),
                ( 2,  1,  495),
                ( 3,  1,  594),
                ( 4,  1,  495),
                ( 5,  1,  347),
                ( 6,  1,  347),
                ( 7,  1,  347),
                ( 8,  1,  495),
                ( 9,  1,  495),
                (10,  1,  594),
                (11,  1,  495),
                (12,  1, 5),
                (13,  1, 5),
                (14,  1, 5),
                (15,  1, 5),
                (16,  1, 4.32692307692f),
                (17,  1, 2),
                (18,  1, 2),
                (19,  1, 2),
                (20,  1, 2),
                (21,  1, 2),
                (22,  1, 2),
                (23,  1, 2),
                (24,  1, 2),
                (25,  1, 2),
                (26,  1, 2),
                (27,  1, 2),
                (28,  1, 2),
                (29,  1, 2),
                (30,  1, 2),
                (31,  1, 2),
                (32,  1, 2),
                (33,  1, 2),
                (34,  1, 2),
                (35,  1, 2),

                ( 2,  2,    1),
                ( 3,  3,    1),
                ( 4,  4,    1),
                ( 5,  5,    1),
                ( 6,  6,    1),
                ( 7,  7,    1),
                ( 8,  8,    1),
                ( 9,  9,    1),
                (10, 10,    1),
                (11, 11,    1),
                (12, 12,    1),
                (13, 13,    1),
                (14, 14,    1),
                (15, 15,    1),
                (16, 16,    1),
                (11, 17,   10),
                (12, 17,   10),
                (13, 17,   2.5f),
                (14, 17,   10),
                (15, 17,   10),
                (16, 16,   1),
                (17, 18,   1),
                (18, 19,   1),
                (19, 20,   1),
                (20, 21,   1),
                (21, 22,   1),
                (22, 23,   1),
                (23, 24,   1),
                (24, 25,   1),
                (25, 26,   1),
                (26, 27,   1),
                (27, 28,   1),
                (28, 29,   1),
                (29, 30,   1),
                (30, 31,   1),
                (31, 32,   1),
                (32, 33,   1),
                (33, 34,   1),
                (34, 35,   1),
                (35, 17,   1)
            };
        }

        ///<inheritdoc cref="IIngrToResDatabase.GetResourceIDs"/>
        public Dictionary<int, float> GetResourceIDs(Ingredient ingredient)
        {
            IEnumerable<(int, float)> result =
                from (int ingredientID, int ResourceID, float Ratio) x in requiresTable
                where x.ingredientID == ingredient.ID
                select (x.ResourceID, x.Ratio);

            return result.ToDictionary(x => x.Item1, x => x.Item2);
        }
    }
}
