using System;
using System.Collections.Generic;

namespace IngredientLists
{
    public class IngredientList
    {
        // dictionary with ingredients and their respective quantities and chosen quantity types
        public Dictionary<Ingredient, (float, QuantityType)> Ingredients { get; private set; }

        public string ListName { get; private set; }

        public IngredientList(string listName, Dictionary<Ingredient, (float, QuantityType)> ingredients = null)
        {
            this.ListName = listName;

            if (ingredients != null)
                this.Ingredients = ingredients;
            else
                this.Ingredients = new();
        }

        public float GetQuantity(Ingredient ingredient)
        {
            (float quantity, _) = Ingredients[ingredient];
            return quantity;
        }

        public QuantityType GetQuantityType(Ingredient ingredient)
        {
            (_, QuantityType type) = Ingredients[ingredient];
            return type;
        }

        public int NumberOfIngredients()
        {
            return Ingredients.Count;
        }
        public void ChangeName(string name)
        {
            ListName = name;
        }

        public void AddIngredient(Ingredient ingredient, float quantity, QuantityType type = QuantityType.G)
        {
            Ingredients.Add(ingredient, (quantity, type));
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }

        public void UpdateIngredient(Ingredient ingredient, float quantity, QuantityType type)
        {
            Ingredients[ingredient] = (quantity, type);
        }
    }

    public class Ingredient : IEquatable<Ingredient>
    {
        public int ID { get; }
        public string Name { get; }

        // the amount of grams that go into one ML/piece of this ingredient;
        // is zero if the conversion is not possible
        public float? GramsPerML { get; }
        public float? GramsPerPiece { get; }

        public Ingredient(int id, string name, float? gramsPerML = null, float? gramsPerPiece = null)
        {
            this.ID = id;
            this.Name = name;
            this.GramsPerML = gramsPerML;
            this.GramsPerPiece = gramsPerPiece;
        }

        // whether ML is a valid quantity type for this ingredient
        public bool MLQuantityPossible()
        {
            return !(GramsPerML == null);
        }

        // whether pieces is a valid quantity type for this ingredient
        public bool PieceQuantityPossible()
        {
            return !(GramsPerPiece == null);
        }

        // Converts the given quantity to the number of grams of this ingredient, given the quantity type
        public float GetNumberOfGrams(float quantity, QuantityType fromType)
        {
            switch (fromType)
            {
                case QuantityType.ML:
                    if (GramsPerML != null)
                        return quantity * GramsPerML.GetValueOrDefault();
                    else
                        throw new NullReferenceException("QuantityType.ML does not exist for this Ingredient");
                case QuantityType.PCS:
                    if (GramsPerPiece != null)
                        return quantity * GramsPerPiece.GetValueOrDefault();
                    else
                        throw new NullReferenceException("QuantityType.PCS does not exist for this Ingredient");
                default:
                    return quantity;
            }
        }

        public override bool Equals(object obj) => obj is Ingredient m && this.Equals(m);

        public bool Equals(Ingredient m) => ID == m.ID;

        public override int GetHashCode() => ID.GetHashCode();

    }

    public enum QuantityType
    {
        G,   // grams
        PCS, // pieces
        ML   // millilitres
    }
}