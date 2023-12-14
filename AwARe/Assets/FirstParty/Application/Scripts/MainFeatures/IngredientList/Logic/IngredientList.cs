using System;
using System.Collections.Generic;

namespace AwARe.IngredientList.Logic
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

        /// <summary>
        /// Represents a single entry in the ingredient list.
        /// This implementation allows expansion without breaking dependent types and members.
        /// </summary>
        public class Entry
        {
            // Contents/Data
            public Ingredient ingredient;
            public float quantity;
            public QuantityType type;

            /// <summary>
            /// Constructs a Ingredient List entry.
            /// </summary>
            /// <param name="ingredient">The ingredient component.</param>
            /// <param name="quantity">The quantity component.</param>
            /// <param name="type">The type of the quantity.</param>
            public Entry(Ingredient ingredient, float quantity, QuantityType type)
            {
                this.ingredient = ingredient;
                this.quantity = quantity;
                this.type = type;
            }

            /// <summary>
            /// Converts tuples to explicit entry types.
            /// </summary>
            /// <param name="entry">The tuple representing on entry.</param>
            public static implicit operator Entry((Ingredient, float, QuantityType) entry) =>
                new(entry.Item1, entry.Item2, entry.Item3);
            
            /// <summary>
            /// Converts entry types to tuple form.
            /// </summary>
            /// <param name="entry">The entry instance.</param>
            public static implicit operator (Ingredient, float, QuantityType)(Entry entry) =>
                (entry.ingredient, entry.quantity, entry.type);
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
        
        /// <summary>
        /// Verifies if the given quantity type is valid for this ingredient.
        /// </summary>
        /// <param name="type">The quantity type</param>
        /// <returns>True if quantity type is valid.</returns>
        public bool QuantityPossible(QuantityType type) =>
            type switch
            {
                QuantityType.G   => true,
                QuantityType.ML  => GramsPerML != null,
                QuantityType.PCS => GramsPerPiece != null,
                _                => false
            };

        // whether ML is a valid quantity type for this ingredient
        public bool MLQuantityPossible() =>
            QuantityPossible(QuantityType.ML);

        // whether pieces is a valid quantity type for this ingredient
        public bool PieceQuantityPossible() =>
            QuantityPossible(QuantityType.PCS);

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