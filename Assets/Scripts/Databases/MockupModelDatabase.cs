using System.Collections.Generic;
using System.Linq;
using ModelLists;

namespace Databases
{
    public class MockupModelDatabase : IModelDatabase
    {
        readonly List<Model> ModelTable;

        public MockupModelDatabase()
        {
            ModelTable = new()
            {
                // Placeholder '10' for all float distances
                new Model( 1, ResourceType.Water, @"path",  10, 10, 10, 10, 10),
                new Model( 2, ResourceType.Plant, @"Crops\beet.fbx",  10, 10, 10, 10, 10),
                new Model( 1, ResourceType.Plant, @"Crops\garlic.fbx",  10, 10, 10, 10, 10),
                new Model( 1, ResourceType.Plant, @"Crops\corn.fbx",  10, 10, 10, 10, 10),
                new Model( 1, ResourceType.Animal, @"Animals\ChickenBrown.prefab",  10, 10, 10, 10, 10),
                new Model( 1, ResourceType.Animal, @"Animals\Pig.prefab",  10, 10, 10, 10, 10),
                new Model( 1, ResourceType.Animal, @"Animals\DuckWhite.prefab",  10, 10, 10, 10, 10),
            };
        }

        public Model GetModel(int id)
        {
            return ModelTable.First(x => x.ID == id);
        }

        public List<Model> GetModels(List<int> ids)
        {
            throw new System.Exception("method not implemented");
        }
    }
}

