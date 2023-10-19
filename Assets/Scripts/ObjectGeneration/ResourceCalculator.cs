using Databases;
using IngredientLists;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ResourceLists
{
    public class ResourceCalculator
    {
        [SerializeField] private IIngredientDatabase ingredientDatabase;
        [SerializeField] private IIngredientToResourceDatabase toResourceDatabase;
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

                ResourceList resourceCosts = GetIngredientResources(pair.Key, pair.Value.Item1, pair.Value.Item2);

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
        ResourceList GetIngredientResources(Ingredient ingredient, float qt, QuantityType type)
        {
            Dictionary<int, float> resourceIDs = toResourceDatabase.GetResourceIDs(ingredient);

            float quantity = ingredient.GetNumberOfGrams(qt, type);

            ResourceList resourceCosts = new();

            foreach (var keyValuePair in resourceIDs)
            {
                // convert resourceID to resource
                Resource resource = resourceDatabase.GetResource(keyValuePair.Key);

                // calculate total resource cost for the quantity (qt) of this ingredient
                float resourceQuantity = keyValuePair.Value * quantity;

                resourceCosts.AddResource(resource, resourceQuantity);
            }

            return resourceCosts;
        }
    }
}