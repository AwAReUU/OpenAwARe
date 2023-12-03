using System.Collections.Generic;

using AwARe.Database;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;

namespace AwARe.ResourcePipeline.Logic
{
    /// <summary>
    /// Class <c>ResourceCalculator</c> converts ingredients to resources by using data from databases.
    /// </summary>
    public class ResourceCalculator
    {
        private readonly IIngrToResDatabase toResourceDatabase;
        private readonly IResourceDatabase resourceDatabase;

        public ResourceCalculator()
        {
            toResourceDatabase = new MockupIngrToResDatabase();
            resourceDatabase = new MockupResourceDatabase();
        }

        /// <summary>
        /// Converts a full <see cref="IngredientList"/> to a <see cref="ResourceDictionary"/>.
        /// It aggregates resource quantities if multiple ingredients contain the same resource,
        /// so that each resource occurs only once in the <see cref="ResourceDictionary"/>.
        /// </summary>
        /// <param name="ingredientList">ingredientList to obtain all resources from.</param>
        /// <returns>ResourceList obtained by converting the given ingredientList</returns>
        public ResourceDictionary IngredientsToResources(IngredientList.Logic.IngredientList ingredientList)
        {
            Dictionary<Resource, float> combinedResourceCosts = new();

            foreach ((Ingredient ingredient, (float ingredientQuantity, QuantityType quantityType)) 
                     in ingredientList.Ingredients)
            {
                ResourceDictionary resourceCosts = GetIngredientResources(ingredient, ingredientQuantity, quantityType);

                foreach ((Resource resource, float resourceQuantity) in resourceCosts.Resources)
                {
                    //Aggregate quantities if resource already present in ResourceList.
                    if (!combinedResourceCosts.ContainsKey(resource))
                        combinedResourceCosts[resource] = resourceQuantity;
                    else
                        combinedResourceCosts[resource] += resourceQuantity;
                }
            }

            return new ResourceDictionary(combinedResourceCosts);
        }

        /// <summary>
        /// Converts a single ingredient to a list of its resource cost.
        /// </summary>
        /// <param name="ingredient">Ingredient to obtain resources from.</param>
        /// <param name="qt">Quantity of the given Ingredient.</param>
        /// <param name="type">QuantityType of the given ingredient.</param>
        /// <returns>All resources needed for given <see cref="Ingredient"/>, in the form of <see cref="ResourceDictionary"/></returns>
        private ResourceDictionary GetIngredientResources(Ingredient ingredient, float qt, QuantityType type)
        {
            //Get all resourceIds and quantities needed for this ingredient.
            Dictionary<int, float> resourceQuantities = toResourceDatabase.GetResourceIDs(ingredient);
            float ingredientQuantityGrams = ingredient.GetNumberOfGrams(qt, type);

            ResourceDictionary resourceCosts = new();

            foreach ((int resourceId, float resourceQuantityPerIngredientGram) in resourceQuantities)
            {
                Resource resource = resourceDatabase.GetResource(resourceId);
                float totalResourceQuantity = resourceQuantityPerIngredientGram * ingredientQuantityGrams;
                resourceCosts.AddResource(resource, totalResourceQuantity);
            }

            return resourceCosts;
        }
    }
}