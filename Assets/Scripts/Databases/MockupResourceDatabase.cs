using System.Collections.Generic;
using ResourceLists;

namespace Databases
{
    public class MockupResourceDatabase : IResourceDatabase
    {
        readonly Dictionary<int, Resource> database;

        public MockupResourceDatabase()
        {
            database = new()
        {
            { 0, new Resource(0, "apple", ResourceType.Plant, 60, 1)},
            { 1, new Resource(1, "wheat", ResourceType.Plant, 20, 3)},
            { 2, new Resource(2, "carrot", ResourceType.Plant, 50, 5)},
            { 3, new Resource(3, "cow", ResourceType.Animal, 3000, 10)},
            { 4, new Resource(4, "pig", ResourceType.Animal, 2500, 11)},
            { 5, new Resource(5, "chicken", ResourceType.Animal, 1000, 12)},
            { 6, new Resource(6, "water", ResourceType.Water, 100, 8)}
        };
        }

        public Resource GetResource(int id)
        {
            return database[id];
        }

        public List<Resource> GetResources(List<int> ids)
        {
            throw new System.Exception("method not implemented");
        }
    }
}

