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
        [SerializeField] private IIngrToResDatabase toResourceDatabase;
        [SerializeField] private IResourceDatabase resourceDatabase;

        public ResourceCalculator()
        {
            toResourceDatabase = new MockupIngrToResDatabase();
            resourceDatabase = new MockupResourceDatabase();
        }

        // Converts a full list of ingredients to a list of the total resource cost
        public ResourceList IngredientsToResources(IngredientList ingredientList)
        {
            Dictionary<Resource, float> combinedResourceCosts = new();

            // loop over all ingredients
            foreach (var pair in ingredientList.Ingredients)
            {
                ResourceList resourceCosts = GetIngredientResources(pair.Key, pair.Value.Item1, pair.Value.Item2);

                // loop over this ingredient's resources
                foreach (var keyValuePair in resourceCosts.Resources)
                {
                    Resource resource = keyValuePair.Key;
                    float quantity = keyValuePair.Value;

                    // check whether the resource already exists in the combined dictionary
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

        // Converts a single ingredient to a list of its resource cost.
        ResourceList GetIngredientResources(Ingredient ingredient, float qt, QuantityType type)
        {
            Dictionary<int, float> resourceIDs = toResourceDatabase.GetResourceIDs(ingredient);

            // convert the saved quantity to its equivalent in grams
            float quantity = ingredient.GetNumberOfGrams(qt, type);

            ResourceList resourceCosts = new();

            foreach (var keyValuePair in resourceIDs)
            {
                // convert resourceID to resource
                Resource resource = resourceDatabase.GetResource(keyValuePair.Key);

                // calculate total resource cost for the quantity of this ingredient
                float resourceQuantity = keyValuePair.Value * quantity;

                resourceCosts.AddResource(resource, resourceQuantity);
            }

            return resourceCosts;
        }
    }
}