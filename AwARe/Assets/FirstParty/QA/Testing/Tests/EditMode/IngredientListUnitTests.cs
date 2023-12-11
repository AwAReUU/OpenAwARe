using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using AwARe.IngredientList.Logic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    /// <summary>
    /// Test regarding the <c>Ingredient</c> object class.
    /// </summary>
    public class IngredientTests
    {
        [Test, Description("Type conversion to ML is not possible if gramsPerML is null.")]
        [TestCase(null, ExpectedResult = false)]
        [TestCase(-1f, ExpectedResult = true)]
        [TestCase(0f, ExpectedResult = true)]
        [TestCase(1f, ExpectedResult = true)]
        public bool TestIngredientMLQuantityPossible(float? gramsPerML)
        {
            //Arrange: Create a Test Ingredient with the given conversion rate.
            Ingredient ingredient = new Ingredient(1, "test", gramsPerML: gramsPerML);

            //Act and Assert: Return MLQuantityPossible and assert that it equals expected result.
            return ingredient.MLQuantityPossible();
        }

        [Test, Description("Type conversion to ML is not possible if gramsPerML is null.")]
        [TestCase(null, ExpectedResult = false)]
        [TestCase(-1f, ExpectedResult = true)]
        [TestCase(0f, ExpectedResult = true)]
        [TestCase(1f, ExpectedResult = true)]
        public bool TestIngredientPieceQuantityPossible(float? gramsPerPiece)
        {
            //Arrange: Create a Test Ingredient with the given conversion rate.
            Ingredient ingredient = new Ingredient(1, "test", gramsPerPiece: gramsPerPiece);

            //Act and Assert: Return PieceQuantityPossible and assert that it equals expected result.
            return ingredient.PieceQuantityPossible();
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly converts quantity from ML to G.")]
        [TestCase(0f, 1f, ExpectedResult = 0f)]
        [TestCase(1f, 0f, ExpectedResult = 0f)]
        [TestCase(1f, 1f, ExpectedResult = 1f)]
        public float TestGetGramsFromML(float quantity, float gramsPerML)
        {
            //Arrange: Create an ingredient with the given gramsPerML conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerML: gramsPerML);

            //Act and Assert:
            return ingr.GetNumberOfGrams(quantity, QuantityType.ML);
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly throws an exception if gramsPerML is null.")]
        public void TestGetGramsFromMLNull()
        {
            Ingredient ingr = new Ingredient(1, "test", gramsPerML: null);

            Assert.Throws<NullReferenceException>(delegate {
                ingr.GetNumberOfGrams(1f, QuantityType.ML);
            });
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly converts quantity from ML to G.")]
        [TestCase(0f, 1f, ExpectedResult = 0f)]
        [TestCase(1f, 0f, ExpectedResult = 0f)]
        [TestCase(1f, 1f, ExpectedResult = 1f)]
        public float TestGetGramsFromPCS(float quantity, float gramsPerPiece)
        {
            //Arrange: Create an ingredient with the given gramsPerML conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerPiece: gramsPerPiece);

            //Act and Assert:
            return ingr.GetNumberOfGrams(quantity, QuantityType.PCS);
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly throws an exception if gramsPerML is null.")]
        public void TestGetGramsFromPCSNull()
        {
            Ingredient ingr = new Ingredient(1, "test", gramsPerPiece: null);

            Assert.Throws<NullReferenceException>(delegate {
                ingr.GetNumberOfGrams(1f, QuantityType.PCS);
            });
        }

        [Test, Description("Two Ingredients are Equal if and only if their IDs are Equal")]
        [TestCase(1, "Orange", 1, "Orange", ExpectedResult = true)]
        [TestCase(1, "Orange", 1, "Apple", ExpectedResult = true)]
        [TestCase(1, "Orange", 2, "Orange", ExpectedResult = false)]
        [TestCase(1, "Orange", 2, "Apple", ExpectedResult = false)]
        public bool TestIngredientEquality(int id1, string name1, int id2, string name2)
        {
            //Arrange: Create Ingredients with the given parameters
            Ingredient ingredient1 = new(id1, name1);
            Ingredient ingredient2 = new(id2, name2);

            //Assert: Ingredient 1 Equals Ingredient 2 if and only if id1 == id2.
            return ingredient1.Equals(ingredient2);
        }
    }

    /// <summary>
    /// Test regarding the <c>IngredientList</c> object class.
    /// </summary>
    public class IngredientListTests
    {
        [Test, Description("The IngredientList constructor creates a new, empty Dictionary if no Dictionary is passed.")]
        public void TestIngredientListConstructorNull()
        {
            //Arrange: Create a new IngredientList using the constructor without ingredients.
            IngredientList list = new("testList");

            //Assert: The List.Ingredients is an empty Dictionary.
            Assert.AreEqual(list.Ingredients, new Dictionary<Ingredient, (float, QuantityType)>());
        }

        [Test, Description("IngredientList.GetQuantity() returns the right quantity of the given ingredient.")]
        [TestCase(-100f)]
        [TestCase(0f)]
        [TestCase(100f)]
        public void TestGetQuantity(
            float quantity)
        {
            //Arrange: Create a test Ingredient, add it to a Dictionary with the given quantity in the value,
            //and create an IngredientList with that Dictionary.
            Ingredient ingredient = new(1, "test");
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(quantity,QuantityType.G) }
            };
            IngredientList list = new("testList", ingredients);

            //Assert: list.GetQuantity() returns the same value as the set quantity of the ingredient.
            Assert.AreEqual(quantity, list.GetQuantity(ingredient));
        }

        [Test, Description("IngredientList.GetQuantityType() returns the right quantity type of the given ingredient.")]
        [TestCase(QuantityType.G)]
        [TestCase(QuantityType.ML)]
        [TestCase(QuantityType.PCS)]
        public void TestGetQuantityType(
            QuantityType quantityType)
        {
            //Arrange: Create a test Ingredient, add it to a Dictionary with the given quantity type in the value,
            //and create an IngredientList with that Dictionary.
            Ingredient ingredient = new(1, "test");
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0,quantityType) }
            };
            IngredientList list = new("testList", ingredients);

            //Assert: list.GetQuantityType() returns the same value as the set QuantityType of the ingredient.
            Assert.AreEqual(quantityType, list.GetQuantityType(ingredient));
        }

        [Test, Description("IngredientList.NumberOfIngredients returns IngredientList.ingredients.Count.")]
        [TestCaseSource(nameof(testDictionaries))]
        public void TestNumberOfIngredients(
            Dictionary<Ingredient, (float, QuantityType)> ingredients)
        {
            //Arrange: Create an IngredientList with the given ingredients Dictionary.
            IngredientList list = new("testList", ingredients);

            //Act: calculate the expected and actual number of ingredients in this list.
            int expected = ingredients.Count;
            int actual = list.NumberOfIngredients();

            //Assert: These two values are equal.
            Assert.AreEqual(expected, actual);
        }

        [Test, Description("IngredientList.AddIngredient() properly adds a new ingredient to the Dictionary.")]
        public void TestAddIngredient()
        {
            //Arrange: Create an empty IngredientList and a new Ingredient.
            IngredientList list = new("testList");
            Ingredient ingredient = new(1, "test");

            //Act: Add the Ingredient to the list using the method.
            list.AddIngredient(ingredient, 0);

            //Assert: list.Ingredients now contains the added Ingredient.
            Assert.IsTrue(list.Ingredients.ContainsKey(ingredient));
        }

        [Test, Description("IngredientList.AddIngredient() fails when trying to add an existent Ingredient.")]
        public void TestAddIngredientFail()
        {
            //Arrange: Create an IngredientList with some ingredients
            //and an Ingredient with an ID identical to one of the Ingredients in the list.
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { new(1,"test1"),(0f,QuantityType.G) },
                { new(2,"test2"),(0f,QuantityType.G) }
            };
            IngredientList list = new("testList", ingredients);
            Ingredient ingredient = new(1, "test");

            //Act and Assert: list.AddIngredient() throws an exception when trying to add this ingredient.s
            Assert.Throws<ArgumentException>(delegate { list.AddIngredient(ingredient, 0); });
        }

        [Test, Description("IngredientList.RemoveIngredient() properly removes the given ingredient.")]
        public void TestRemoveIngredient(
            [ValueSource(nameof(testDictionaries))] Dictionary<Ingredient, (float, QuantityType)> ingredients)
        {
            //Arrange: Create an ingredientList with the given ingredients Dictionary,
            //and an Ingredient with an ID that may or may not be part of the dictionary.
            IngredientList list = new("testList", ingredients);
            Ingredient ingredient = new(1, "test");

            //Act: Remove the ingredient from the list.
            list.RemoveIngredient(ingredient);

            //Assert: list.Ingredients does not contain the ingredient, nor the Ingredient that had ID == 1.
            Assert.IsFalse(list.Ingredients.ContainsKey(ingredient));
            Assert.IsFalse(list.Ingredients.ContainsKey(new(1, "test1")));
        }

        [Test, Description("IngredientList.UpdateIngredient() properly sets the dictionary quantity value to a new value.")]
        [TestCase(-1f)]
        [TestCase(0f)]
        [TestCase(1f)]
        public void TestUpdateIngredientQuantity(float newQuantity)
        {
            //Arrange: Create a IngredientList containing an Ingredient (with default values).
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0f,QuantityType.G) }
            };
            IngredientList list = new("testList", ingredients);

            //Act: Update the value at key == ingredient and retrieve the updated quantity.
            list.UpdateIngredient(ingredient, newQuantity, QuantityType.G);
            float actualQuantity = list.GetQuantity(ingredient);

            //Assert: The quantity is set to the new value.
            Assert.AreEqual(newQuantity, actualQuantity);
        }

        [Test, Description("IngredientList.UpdateIngredient() properly sets the dictionary values to their new values.")]
        [TestCase(QuantityType.G)]
        [TestCase(QuantityType.ML)]
        [TestCase(QuantityType.PCS)]
        public void TestUpdateIngredientQuantityType(QuantityType newQuantityType)
        {
            //Arrange: Create a IngredientList containing an Ingredient (with default values).
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0f,QuantityType.G) }
            };
            IngredientList list = new("testList", ingredients);

            //Act: Update the value at key == ingredient and retrieve the updated quantity type.
            list.UpdateIngredient(ingredient, 0f, newQuantityType);
            QuantityType actualQuantityType = list.GetQuantityType(ingredient);

            //Assert: The quantity is set to the new value.s
            Assert.AreEqual(newQuantityType, actualQuantityType);
        }

        [Test, Description("IngredientList.UpdateIngredient() properly handles when updating an ingredient it doesn't contain.")]
        public void TestUpdateNonExistentIngredient()
        {
            //Arrange: Create an Ingredient, an empty IngredientList, and some "new" values.
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new();
            IngredientList list = new("testList", ingredients);
            float newQuantity = 1f;
            QuantityType newType = QuantityType.PCS;

            //Act: Update the value at key == ingredient.
            list.UpdateIngredient(ingredient, newQuantity, newType);

            //Assert: the ingredient is added to the list
            Assert.IsTrue(list.Ingredients.ContainsKey(ingredient));
        }

        /// <summary>
        /// 3 <c>Dictionaries</c> used for testing. 
        /// An empty one; one with 1 key-value pair;
        /// and finally, one with 2 key-value pairs.
        /// </summary>
        private static Dictionary<Ingredient, (float, QuantityType)>[] testDictionaries =
        {
            new(),
            new()
            {
                { new(1,"test1"),(0f,QuantityType.G) },
            },
            new()
            {
                { new(1,"test1"),(0f,QuantityType.G) },
                { new(2,"test2"),(0f,QuantityType.G) }
            },
        };
    }
}