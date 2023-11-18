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

    /// <summary>
    /// Calculate how many models we need to render for the given resource
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="resourceQuantity"></param>
    /// <returns></returns>
    public int CalculateModelQuantity(Resource resource, float resourceQuantity)
    {
        if (resource.GramsPerModel != null) 
            return (int)Math.Ceiling((double)(resourceQuantity / resource.GramsPerModel));
        if (resource.Name == "Water")
            return (int)Math.Ceiling(resourceQuantity / 1500); //1500grams per bottle
        if (resource.Name == "Milk")
            return (int)Math.Ceiling(resourceQuantity / 1000); //1000grams per carton
        return 1;
    }
}