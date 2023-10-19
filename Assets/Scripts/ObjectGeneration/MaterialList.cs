using System;
using System.Collections.Generic;

public class MaterialList
{
    public Dictionary<ProductMaterial, float> Materials { get; private set; }

    public MaterialList(Dictionary<ProductMaterial, float> materials = null)
    {
        if (materials != null)
            this.Materials = materials;
        else
            this.Materials = new();
    }

    public int NumberOfMaterials()
    {
        return Materials.Count;
    }

    public void AddMaterial(ProductMaterial material, float quantity)
    {
        Materials.Add(material, quantity);
    }
}

public class ProductMaterial : IEquatable<ProductMaterial>
{
    public int ID { get; }
    public MaterialType Type { get; }

    public ProductMaterial(int id, MaterialType type)
    {
        ID = id;
    }

    public override bool Equals(object obj) => obj is ProductMaterial m && this.Equals(m);

    public bool Equals(ProductMaterial m) => ID == m.ID;

    public override int GetHashCode() => ID.GetHashCode();

}

public enum MaterialType
{
    Animal,
    Plant,
    Water
}