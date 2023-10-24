using System.Collections.Generic;
using System.Linq;
using IngredientLists;

namespace Databases
{
    public class MockupIngrToResDatabase : IIngrToResDatabase
    {
        private readonly List<(int, int, float)> requiresTable; // (IngrID, ResID, ResGrams per IngrGram)
        
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
                (12,  1, 1000),
                (13,  1, 1000),
                (14,  1, 1000),
                (15,  1, 1000),
                (16,  1, 1000),
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
                (13, 17,   10),
                (14, 17,   10),
                (15, 17,   10),
                (16, 17,   10),
            };
        }
        public Dictionary<int, float> GetResourceIDs(Ingredient ingredient)
        {
            List<(int, float)> result = requiresTable.Where(x => x.Item1 == ingredient.ID).Select(x => (x.Item2,x.Item3)).ToList();
            Dictionary<int, float> resultDictionary = result.ToDictionary(x => x.Item1, x => x.Item2);

            return resultDictionary;
        }
    }
}
