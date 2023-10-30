using System.Collections.Generic;
using System.Linq;
using ResourceLists;

namespace Databases
{
    public class MockupResourceDatabase : IResourceDatabase
    {
        readonly List<Resource> ResourceTable;

        public MockupResourceDatabase()
        {
            ResourceTable = new()
            {
                new Resource( 1,      "Water",  ResourceType.Water,   null, 1),
                new Resource( 2,      "Apple",  ResourceType.Plant,  10000, 7), 
                new Resource( 3,     "Banana",  ResourceType.Plant,  10000, 7),
                new Resource( 4,       "Pear",  ResourceType.Plant,  10000, 7),
                new Resource( 5,   "Mandarin",  ResourceType.Plant,   8000, 7),
                new Resource( 6,     "Orange",  ResourceType.Plant,  10000, 7),
                new Resource( 7,      "Grape",  ResourceType.Plant,   5000, 7),
                new Resource( 8, "Strawberry",  ResourceType.Plant,    100, 7),
                new Resource( 9, "Kiwi Fruit",  ResourceType.Plant,   1000, 7),
                new Resource(10,  "Pineapple",  ResourceType.Plant,   1000, 7),
                new Resource(11,      "Melon",  ResourceType.Plant,   5000, 7),
                new Resource(12,       "Beef", ResourceType.Animal, 200000, 2),
                new Resource(13,    "Chicken", ResourceType.Animal,   2500, 3),
                new Resource(14,       "Pork", ResourceType.Animal,  50000, 4),
                new Resource(15,       "Duck", ResourceType.Animal,   2500, 5),
                new Resource(16,       "Milk", ResourceType.Animal,   null, 2),
                new Resource(17,      "Wheat",  ResourceType.Plant,     80, 6)
            };
        }

        public Resource GetResource(int id)
        {
            return ResourceTable.First(x => x.ID == id);
        }

        public List<Resource> GetResources(IEnumerable<int> ids)
        {
            return ResourceTable.Where(x => ids.Contains(x.ID)).ToList();
        }
    }
}

