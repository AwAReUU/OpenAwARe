// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwARe.ResourcePipeline.Logic;

namespace AwARe.Database.Logic
{
    /// <summary>
    /// Implementation of the Resource Database interface, that uses mock database using a locally saved table.
    /// The Mock Database can be used for testing purposes.
    /// </summary>
    public class MockupResourceDatabase : IResourceDatabase
    {
        readonly List<Resource> resourceTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockupResourceDatabase"/> class.
        /// </summary>
        public MockupResourceDatabase()
        {
            resourceTable = new()
            {
                new Resource( 1,      "Water", ResourceType.Water,    null,  1),
                new Resource( 2,      "Apple", ResourceType.Plant,   10000,  6),
                new Resource( 3,     "Banana", ResourceType.Plant,   10000,  6),
                new Resource( 4,       "Pear", ResourceType.Plant,   10000,  6),
                new Resource( 5,   "Mandarin", ResourceType.Plant,    8000,  6),
                new Resource( 6,     "Orange", ResourceType.Plant,   10000,  6),
                new Resource( 7,      "Grape", ResourceType.Plant,    5000,  6),
                new Resource( 8, "Strawberry", ResourceType.Plant,     100,  6),
                new Resource( 9, "Kiwi Fruit", ResourceType.Plant,    1000,  6),
                new Resource(10,  "Pineapple", ResourceType.Plant,    1000,  6),
                new Resource(11,      "Melon", ResourceType.Plant,    5000,  6),
                new Resource(12,       "Beef", ResourceType.Animal, 200000,  2),
                new Resource(13,    "Chicken", ResourceType.Animal,   2500,  3),
                new Resource(14,       "Pork", ResourceType.Animal,  50000,  4),
                new Resource(15,       "Duck", ResourceType.Animal,   2500,  5),
                new Resource(16,       "Milk", ResourceType.Animal,   1000,  8),
                new Resource(17,     "Potato", ResourceType.Plant,    1000,  9),
                new Resource(18,       "Beet", ResourceType.Plant,     250, 10),
                new Resource(19,  "Artichoke", ResourceType.Plant,     300, 11),
                new Resource(20,   "Broccoli", ResourceType.Plant,     500, 12),
                new Resource(21,    "Cabbage", ResourceType.Plant,    1000, 13),
                new Resource(22,     "Carrot", ResourceType.Plant,     550, 14),
                new Resource(23,       "Corn", ResourceType.Plant,    2000, 15),
                new Resource(24,   "Cucumber", ResourceType.Plant,     800, 17),
                new Resource(25,   "Eggplant", ResourceType.Plant,    1000, 18),
                new Resource(26,     "Garlic", ResourceType.Plant,     150, 19),
                new Resource(27,      "Onion", ResourceType.Plant,     250, 21),
                new Resource(28,     "Pepper", ResourceType.Plant,     400, 22),
                new Resource(29,      "Poppy", ResourceType.Plant,     100, 23),
                new Resource(30,    "Pumpkin", ResourceType.Plant,    2000, 24),
                new Resource(31,  "Sunflower", ResourceType.Plant,    1000, 25),
                new Resource(32,     "Tomato", ResourceType.Plant,     800, 26),
                new Resource(33,      "Wheat", ResourceType.Plant,    1000,  7)
            };
        }

        ///<inheritdoc cref="IResourceDatabase.GetResource"/>
        public Task<Resource> GetResource(int id)
        {
            // return the first resource that matches id, should be the only one since IDs are unique
            return Task.Run(() => resourceTable.First(x => x.ID == id));
        }

        ///<inheritdoc cref="IResourceDatabase.GetResources"/>
        public Task<Resource[]> GetResources(IEnumerable<int> ids)
        {
            // perform an inner join of resource table and ids on resourceID
            IEnumerable<Resource> resources =
                from id in ids
                join resource in resourceTable on id equals resource.ID
                select resource;
            return Task.Run(() => resources.ToArray());
        }
    }
}

