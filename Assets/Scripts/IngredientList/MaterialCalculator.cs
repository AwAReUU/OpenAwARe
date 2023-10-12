using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCalculator
{
    MockupIngredientDatabase ingredientDatabase;

    public Dictionary<ProductMaterial, float> IngredientsToMaterials(IngredientList ingredientList)
    {
        Dictionary<ProductMaterial, float> combinedMaterialCosts = new();

        foreach (var pair in ingredientList.Ingredients)
        {

            Dictionary<ProductMaterial, float> materialCosts = GetIngredientMaterials(pair.Key, pair.Value);

            foreach (var keyValuePair in materialCosts)
            {
                ProductMaterial material = keyValuePair.Key;
                float quantity = keyValuePair.Value;

                if (!combinedMaterialCosts.ContainsKey(material))
                {
                    combinedMaterialCosts[material] = quantity;
                }
                else
                {
                    combinedMaterialCosts[material] += quantity;
                }
            }
        }

        return combinedMaterialCosts;
    }

    Dictionary<ProductMaterial, float> GetIngredientMaterials(Ingredient ingredient, float quantity)
    {
        Dictionary<ProductMaterial, float> materialsPerQuantity = ingredientDatabase.GetMaterials(ingredient);

        Dictionary<ProductMaterial, float> materialCosts = new();

        foreach (var keyValuePair in materialsPerQuantity)
        {
            ProductMaterial material = keyValuePair.Key;
            materialCosts[material] = keyValuePair.Value * quantity;
        }

        return materialCosts;
    }
}