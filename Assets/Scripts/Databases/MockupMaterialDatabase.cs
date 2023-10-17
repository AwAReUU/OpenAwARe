using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockupMaterialDatabase : IMaterialDatabase
{
    Dictionary<int, ProductMaterial> database;

    public MockupMaterialDatabase()
    {
        database = new()
        {
            { 0, new ProductMaterial(0, MaterialType.Plant)},
            { 1, new ProductMaterial(1, MaterialType.Plant)},
            { 2, new ProductMaterial(2, MaterialType.Plant)},
            { 3, new ProductMaterial(3, MaterialType.Animal)},
            { 4, new ProductMaterial(4, MaterialType.Animal)},
            { 5, new ProductMaterial(5, MaterialType.Animal)},
            { 6, new ProductMaterial(6, MaterialType.Water)}
        };
    }

    public ProductMaterial GetMaterial(int id)
    {
        return database[id];
    }
}
