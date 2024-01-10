// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;

namespace AwARe.IngredientList.Logic
{
    /// <summary>
    /// Data structure that stores a list name and a Dictionary of <see cref="Ingredient"/>s with their quantity and <see cref="QuantityType"/>.
    /// </summary>
    public class IngredientList
    {
        /// <summary>
        /// Gets the Dictionary with ingredients and their respective quantities and chosen quantity types.
        /// </summary>
        /// <value>A dictionary with Ingredient as key and their quantity(float) and QuantityType as value.</value>
        public Dictionary<Ingredient, (float, QuantityType)> Ingredients { get; private set; }

        /// <summary>
        /// Gets the name given to this IngredientList.
        /// </summary>
        /// <value>The name currently given to this list.</value>
        public string ListName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IngredientList"/> class.
        /// </summary>
        /// <param name="listName">The name of this IngredientList.</param>
        /// <param name="ingredients">Dictionary of Ingredients and their quantities, creates a new empty dictionary if null.</param>
        public IngredientList(string listName, Dictionary<Ingredient, (float, QuantityType)> ingredients = null)
        {
            this.ListName = listName;

            if (ingredients != null)
                this.Ingredients = ingredients;
            else
                this.Ingredients = new();
        }

        /// <summary>
        /// Gets the current quantity of the given ingredient from the Dictionary.
        /// </summary>
        /// <param name="ingredient">The ingredient to retrieve the quantity of.</param>
        /// <returns>The current quantity of the ingredient.</returns>
        public float GetQuantity(Ingredient ingredient)
        {
            (float quantity, _) = Ingredients[ingredient];
            return quantity;
        }

        /// <summary>
        /// Gets the current quantity type of the given ingredient from the Dictionary.
        /// </summary>
        /// <param name="ingredient">The ingredient to retrieve the quantity type of.</param>
        /// <returns>The current quantity type of the ingredient.</returns>
        public QuantityType GetQuantityType(Ingredient ingredient)
        {
            (_, QuantityType type) = Ingredients[ingredient];
            return type;
        }

        /// <summary>
        /// Gets the current number of ingredients added to the IngredientList.
        /// </summary>
        /// <returns>The number of KeyValuePairs in the ingredient Dictionary.</returns>
        public int NumberOfIngredients()
        {
            return Ingredients.Count;
        }

        /// <summary>
        /// Sets the name of the <see cref="IngredientList"/> to the given name.
        /// </summary>
        /// <param name="name">The new ListName.</param>
        public void ChangeName(string name)
        {
            ListName = name;
        }

        /// <summary>
        /// Adds a new KeyValuePair to the Dictionary.
        /// </summary>
        /// <param name="ingredient">The Ingredient that is added.</param>
        /// <param name="quantity">The quantity of the added ingredient.</param>
        /// <param name="type">The quantity type of the added ingredient, in grams if left null.</param>
        public void AddIngredient(Ingredient ingredient, float quantity, QuantityType type = QuantityType.G)
        {
            Ingredients.Add(ingredient, (quantity, type));
        }

        /// <summary>
        /// Removes the given ingredient from the Dictionary.
        /// </summary>
        /// <param name="ingredient">The Ingredient Key that is removed.</param>
        public void RemoveIngredient(Ingredient ingredient)
        {
            Ingredients.Remove(ingredient);
        }

        /// <summary>
        /// Updates the quantity and type Value in the Dictionary of the given Ingredient.
        /// </summary>
        /// <param name="ingredient">The Key of which the value is updated.</param>
        /// <param name="quantity">The new quantity of the ingredient.</param>
        /// <param name="type">The new quantity type of the ingredient.</param>
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

    /// <summary>
    /// Class that stores Ingredient data retrieved from the Ingredient Database.
    /// </summary>
    public class Ingredient : IEquatable<Ingredient>
    {
        /// <summary>
        /// Gets the Unique Identifier of the Ingredient. Matches with the one in the Database.
        /// </summary>
        /// <value>The IngredientID of the Ingredient.</value>
        public int ID { get; }

        /// <summary>
        /// Gets the Name of the Ingredient that is displayed throughout the UI.
        /// </summary>
        /// <value>The name of the Ingredient.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the rate of converting the quantity from grams to millilitres. Null means conversion is impossible.
        /// </summary>
        /// <value>The number of grams per ml of the Ingredient.</value>
        public float? GramsPerML { get; }

        /// <summary>
        /// Gets the rate of converting the quantity from grams to pieces. Null means conversion is impossible.
        /// </summary>
        /// <value>The number of grams per piece of the Ingredient.</value>
        public float? GramsPerPiece { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ingredient"/> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <param name="name">The name of this ingredient.</param>
        /// <param name="gramsPerML">The grams per ml conversion rate.</param>
        /// <param name="gramsPerPiece">The grams per piece conversion rate.</param>
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

        /// <summary>
        /// Checks whether ML is a valid quantity type for this ingredient.
        /// </summary>
        /// <returns>true if conversion is possible, otherwise false.</returns>
        public bool MLQuantityPossible() =>
            QuantityPossible(QuantityType.ML);

        /// <summary>
        /// Checks whether PCS is a valid quantity type for this ingredient.
        /// </summary>
        /// <returns>true if conversion is possible, otherwise false.</returns>
        public bool PieceQuantityPossible() =>
            QuantityPossible(QuantityType.PCS);

        /// <summary>
        /// Calculates the number of grams in the given quantity.
        /// </summary>
        /// <param name="quantity">The quantity to be converted.</param>
        /// <param name="fromType">The QuantityType in which the given quantity is measured.</param>
        /// <returns>The given quantity, converted from the given QuantityType to grams.</returns>
        /// <exception cref="NullReferenceException">Thrown when trying to convert from a type where the conversion rate is null.</exception>
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

        /// <summary>
        /// Checks equality between this Ingredient and any other object.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>true if object is an Ingredient with the same ID, otherwise false.</returns>
        public override bool Equals(object obj) => obj is Ingredient m && this.Equals(m);

        /// <summary>
        /// Checks equality between this Ingredient and another Ingredient.
        /// </summary>
        /// <param name="m">The Ingredient to compare to.</param>
        /// <returns>true if the Ingredients have the same ID, otherwise false.</returns>
        public bool Equals(Ingredient m) => ID == m.ID;

        /// <summary>
        /// Gets the Hash Code of the Ingredient.
        /// </summary>
        /// <returns>The Hash Code of the Ingredient.</returns>
        public override int GetHashCode() => ID.GetHashCode();
    }

    /// <summary>
    /// Enum with every possible value of QuantityType, aka the possible units for measuring ingredient quantities.
    /// </summary>
    public enum QuantityType
    {
        G,   // grams
        PCS, // pieces
        ML   // millilitres
    }
}