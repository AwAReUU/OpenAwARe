using System.Collections.Generic;
using System.Linq;

using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database.Logic
{
    public class MockupResourceDatabase : IResourceDatabase
    {
        readonly List<Resource> resourceTable;

        public MockupResourceDatabase()
        {
            resourceTable = new()
            {
                new Resource( 1,      "Water",  ResourceType.Water,    null, 1),
                new Resource( 2,      "Apple",  ResourceType.Plant,   10000, 7), 
                new Resource( 3,     "Banana",  ResourceType.Plant,   10000, 7),
                new Resource( 4,       "Pear",  ResourceType.Plant,   10000, 7),
                new Resource( 5,   "Mandarin",  ResourceType.Plant,    8000, 7),
                new Resource( 6,     "Orange",  ResourceType.Plant,   10000, 7),
                new Resource( 7,      "Grape",  ResourceType.Plant,    5000, 6),
                new Resource( 8, "Strawberry",  ResourceType.Plant,     100, 7),
                new Resource( 9, "Kiwi Fruit",  ResourceType.Plant,    1000, 7),
                new Resource(10,  "Pineapple",  ResourceType.Plant,    1000, 7),
                new Resource(11,      "Melon",  ResourceType.Plant,    5000, 7),
                new Resource(12,       "Beef",  ResourceType.Animal, 200000, 2),
                new Resource(13,    "Chicken",  ResourceType.Animal,   2500, 3),
                new Resource(14,       "Pork",  ResourceType.Animal,  50000, 4),
                new Resource(15,       "Duck",  ResourceType.Animal,   2500, 5),
                new Resource(16,       "Milk",  ResourceType.Animal,   null, 8),
                new Resource(17,      "Wheat",  ResourceType.Plant,      1000, 7),
                new Resource(18,     "Potato",  ResourceType.Plant,    1000, 9),
                new Resource(19,     "Beet",  ResourceType.Plant,     250, 10)
            };
        }

        public Resource GetResource(int id)
        {
            // return the first resource that matches id, should be the only one since IDs are unique
            return resourceTable.First(x => x.ID == id);
        }

        public List<Resource> GetResources(IEnumerable<int> ids)
        {
            // perform an inner join of resource table and ids on resourceID
            IEnumerable<Resource> resources =
                from id in ids
                join resource in resourceTable on id equals resource.ID
                select resource;
            return resources.ToList();
        }
    }
}

