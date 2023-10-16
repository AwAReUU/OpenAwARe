using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialCalculator
{
    [SerializeField] private IIngredientDatabase ingredientDatabase;
    [SerializeField] private IMaterialDatabase materialDatabase;

    public MaterialList IngredientsToMaterials(IngredientList ingredientList)
    {
        Dictionary<ProductMaterial, float> combinedMaterialCosts = new();

        foreach (var pair in ingredientList.Ingredients)
        {

            MaterialList materialCosts = GetIngredientMaterials(pair.Key, pair.Value);

            foreach (var keyValuePair in materialCosts.Materials)
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

        return new MaterialList(combinedMaterialCosts);
    }

    MaterialList GetIngredientMaterials(Ingredient ingredient, float qt)
    {
        Dictionary<int, float> materialIDs = ingredientDatabase.GetMaterialIDs(ingredient);

        MaterialList materialCosts = new();

        foreach (var keyValuePair in materialIDs)
        {
            // convert materialID to material
            ProductMaterial material = materialDatabase.GetMaterial(keyValuePair.Key);

            // calculate total material cost for the quantity (qt) of this ingredient
            float quantity = keyValuePair.Value * qt;

            materialCosts.AddMaterial(material, quantity);
        }

        return materialCosts;
    }
}