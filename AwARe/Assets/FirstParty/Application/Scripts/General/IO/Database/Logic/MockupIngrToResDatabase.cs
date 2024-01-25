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
                (16, 17,   1.25f),
                (17, 18,   1),
                (17, 1,    2),
                (18, 19,   1),
                (18, 1,    2)
            };
        }

        ///<inheritdoc cref="IIngrToResDatabase.GetResourceIDs"/>
        public Task<Dictionary<int, float>> GetResourceIDs(Ingredient ingredient)
        {
            IEnumerable<(int, float)> result =
                from (int ingredientID, int ResourceID, float Ratio) x in requiresTable
                where x.ingredientID == ingredient.IngredientID
                select (x.ResourceID, x.Ratio);

            return Task.Run(() => result.ToDictionary(x => x.Item1, x => x.Item2));
        }
    }
}
