using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCalculator
{
    [SerializeField] private IIngredientDatabase ingredientDatabase;
    [SerializeField] private IResourceDatabase resourceDatabase;

    public ResourceCalculator()
    {  
        ingredientDatabase = new MockupIngredientDatabase();
        resourceDatabase = new MockupResourceDatabase();
    }

    public ResourceList IngredientsToResources(IngredientList ingredientList)
    {
        Dictionary<Resource, float> combinedResourceCosts = new();

        foreach (var pair in ingredientList.Ingredients)
        {

            ResourceList resourceCosts = GetIngredientResources(pair.Key, pair.Value);

            foreach (var keyValuePair in resourceCosts.Resources)
            {
                Resource resource = keyValuePair.Key;
                float quantity = keyValuePair.Value;

                // check whether the resource already exists in the dictionary
                if (!combinedResourceCosts.ContainsKey(resource))
                {
                    combinedResourceCosts[resource] = quantity;
                }
                else
                {
                    combinedResourceCosts[resource] += quantity;
                }
            }
        }

        return new ResourceList(combinedResourceCosts);
    }

    
    // gets the list of resources of a single ingredient
    ResourceList GetIngredientResources(Ingredient ingredient, float qt)
    {
        Dictionary<int, float> resourceIDs = ingredientDatabase.GetResourceIDs(ingredient);

        ResourceList resourceCosts = new();

        foreach (var keyValuePair in resourceIDs)
        {
            // convert resourceID to resource
            Resource resource = resourceDatabase.GetResource(keyValuePair.Key);

            // calculate total resource cost for the quantity (qt) of this ingredient
            float quantity = keyValuePair.Value * qt;

            resourceCosts.AddResource(resource, quantity);
        }

        return resourceCosts;
    }
}