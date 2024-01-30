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
                (12,  1, 15415),
                (13,  1, 4325),
                (14,  1, 5988),
                (15,  1, 4325),
                (16,  1, 4.32692307692f),
                (17,  1,  347),
                (18,  1,  347),
                (19,  1,  347),
                (20,  1,  347),
                (21,  1,  347),
                (22,  1,  347),
                (23,  1,  347),
                (24,  1,  347),
                (25,  1,  347),
                (26,  1,  347),
                (27,  1,  347),
                (28,  1,  347),
                (29,  1,  347),
                (30,  1,  347),
                (31,  1,  347),
                (32,  1,  347),
                (33,  1,  347),
                (34,  1,  700),

                (11, 33, 10),
                (12, 33, 10),
                (13, 33, 2.5f),
                (14, 33, 10),
                (15, 33, 2.5f),
                (16, 33, 1),

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
                (17, 17,    1),
                (18, 18,    1),
                (19, 19,    1),
                (20, 20,    1),
                (21, 21,    1),
                (22, 22,    1),
                (23, 23,    1),
                (24, 24,    1),
                (25, 25,    1),
                (26, 26,    1),
                (27, 27,    1),
                (28, 28,    1),
                (29, 29,    1),
                (30, 30,    1),
                (31, 31,    1),
                (32, 32,    1),
                (33, 33,    1),
                (34, 33,    0.95f),
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
