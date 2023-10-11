using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDatabase
{
    public Dictionary<ProductMaterial, float> GetMaterials(Ingredient ingredient)
    {
        Dictionary<ProductMaterial, float> materials = new();
        //TODO: get materials from database
        materials.Add(new ProductMaterial(1, MaterialType.Plant),1);
        materials.Add(new ProductMaterial(3, MaterialType.Water), 0.8f);
        materials.Add(new ProductMaterial(6, MaterialType.Animal), 0.2f);
        return materials;
    }
}