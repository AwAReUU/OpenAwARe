using Databases;
using ResourceLists;
using System;
using UnityEngine;

class ModelCalculator 
{
    [SerializeField] private IResourceDatabase resourceDatabase;
    [SerializeField] private IModelDatabase modelDatabase;

    public ModelCalculator() 
    {
        modelDatabase = new MockupModelDatabase();
        resourceDatabase = new MockupResourceDatabase();
    }

    public int CalculateModelQuantity(Resource resource, float resourceQuantity)
    {
        if (resource.GramsPerModel != null) 
            return (int)Math.Ceiling((double)(resourceQuantity / resource.GramsPerModel));
        if (resource.Name == "Water" || resource.Name == "Milk")
            return 1;
        return 1;
    }
}