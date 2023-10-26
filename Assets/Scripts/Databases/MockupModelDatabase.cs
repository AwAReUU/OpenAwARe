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
                // Placeholder '10' for all float distances (.prefab needs to be removed!)
                new Model( 1, ResourceType.Water, @"cube",  10, 10, 10, 10, 10),
                new Model( 2, ResourceType.Animal,@"Animals/CowBlW",  10, 10, 10, 10, 10),
                new Model( 3, ResourceType.Animal,@"Animals/ChickenBrown",  10, 10, 10, 10, 10), 
                new Model( 4, ResourceType.Animal,@"Animals/Pig",  10, 10, 10, 10, 10),
                new Model( 5, ResourceType.Animal,@"Animals/DuckWhite",  10, 10, 10, 10, 10),
                new Model( 6, ResourceType.Plant, @"Crops/grap",  10, 10, 10, 10, 10),
                new Model( 7, ResourceType.Plant, @"Crops/wheat1",  10, 10, 10, 10, 10),
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

