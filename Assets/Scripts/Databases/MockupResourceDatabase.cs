using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockupResourceDatabase : IResourceDatabase
{
    Dictionary<int, Resource> database;

    public MockupResourceDatabase()
    {
        database = new()
        {
            { 0, new Resource(0, ResourceType.Plant)},
            { 1, new Resource(1, ResourceType.Plant)},
            { 2, new Resource(2, ResourceType.Plant)},
            { 3, new Resource(3, ResourceType.Animal)},
            { 4, new Resource(4, ResourceType.Animal)},
            { 5, new Resource(5, ResourceType.Animal)},
            { 6, new Resource(6, ResourceType.Water)}
        };
    }

    public Resource GetResource(int id)
    {
        return database[id];
    }
}
