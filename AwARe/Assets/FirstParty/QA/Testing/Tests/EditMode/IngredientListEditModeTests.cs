// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections.Generic;
using System.IO;

using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;
using NUnit.Framework;

namespace AwARe.Tests.IngredientList
{
    /// <summary>
    /// Test regarding the <c>Ingredient</c> object class.
    /// </summary>
    public class IngredientTests
    {
        [Test, Description("Can get the ID of the ingredient.")]
        [TestCase(0)]
        [TestCase(1)]
        public void Test_GetID(int id)
        {
            //Arrange: Create an ingredient using the given name.
            Ingredient ingredient = new Ingredient(id, "test");

            //Act and Assert: Get the ingredients name and assert it's the same as the one given to the constructor.
            Assert.AreEqual(id, ingredient.IngredientID);
        }

        [Test, Description("Can get the name of the ingredient.")]
        [TestCase("")]
        [TestCase("test")]
        public void Test_GetName(string name)
        {
            //Arrange: Create an ingredient using the given name.
            Ingredient ingredient = new Ingredient(1, name);

            //Act and Assert: Get the ingredients name and assert it's the same as the one given to the constructor.
            Assert.AreEqual(name, ingredient.PrefName);
        }

        [Test, Description("Type conversion to ML is not possible if gramsPerML is null.")]
        [TestCase(null, ExpectedResult = false)] // gramsPerML = null is used to indicate conversion is not possible.
        [TestCase(-1f, ExpectedResult = true)] // negative gramsPerML is currently possible, but is actually physically impossible IRL.
        [TestCase(0f, ExpectedResult = true)] // gramsPerML = 0 is currently possible, but is also physically impossible IRL.
        [TestCase(1f, ExpectedResult = true)] // positive gramsPerML should always be possible.
        public bool Test_MLQuantityPossible(float? gramsPerML)
        {
            //Arrange: Create a Test Ingredient with the given conversion rate.
            Ingredient ingredient = new Ingredient(1, "test", gramsPerML: gramsPerML);

            //Act and Assert: Return MLQuantityPossible and assert that it equals expected result.
            return ingredient.MLQuantityPossible();
        }

        [Test, Description("Type conversion to ML is not possible if gramsPerML is null.")]
        [TestCase(null, ExpectedResult = false)] // gramsPerPiece = null is used to indicate conversion is not possible.
        [TestCase(-1f, ExpectedResult = true)] // negative gramsPerPiece is currently possible, but is actually physically impossible IRL.
        [TestCase(0f, ExpectedResult = true)] // gramsPerPiece = 0 is currently possible, but is also physically impossible IRL.
        [TestCase(1f, ExpectedResult = true)] // positive gramsPerPiece should always be possible.
        public bool Test_PieceQuantityPossible(float? gramsPerPiece)
        {
            //Arrange: Create a Test Ingredient with the given conversion rate.
            Ingredient ingredient = new Ingredient(1, "test", gramsPerPiece: gramsPerPiece);

            //Act and Assert: Return PieceQuantityPossible and assert that it equals expected result.
            return ingredient.PieceQuantityPossible();
        }

        [Test, Description("Ingredient.GetNumberOfGrams just return the quantity when fromType is grams.")]
        [TestCase(0f, ExpectedResult = 0f)]
        [TestCase(11f, ExpectedResult = 11f)]
        [TestCase(121f, ExpectedResult = 121f)]
        public float Test_GetNumberOfGrams_ML(float quantity)
        {
            //Arrange: Create an ingredient with the given gramsPerML conversion rate.
            Ingredient ingr = new Ingredient(1, "test");

            //Act and Assert:
            return ingr.GetNumberOfGrams(quantity, QuantityType.G);
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly converts quantity from ML to G.")]
        [TestCase(0f, 11f, ExpectedResult = 0f)]
        [TestCase(11f, 0f, ExpectedResult = 0f)]
        [TestCase(11f, 11f, ExpectedResult = 121f)]
        public float Test_GetNumberOfGrams_ML(float quantity, float gramsPerML)
        {
            //Arrange: Create an ingredient with the given gramsPerML conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerML: gramsPerML);

            //Act and Assert:
            return ingr.GetNumberOfGrams(quantity, QuantityType.ML);
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly throws an exception if gramsPerML is null.")]
        public void Test_GetNumberOfGrams_ML_Null()
        {
            //Arrange: Create an ingredient with the given gramsPerML conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerML: null);

            //Act and Assert: Try to get the number of grams using this conversion rate and Assert that it throws an Exception.
            Assert.Throws<NullReferenceException>(delegate
            {
                ingr.GetNumberOfGrams(1f, QuantityType.ML);
            });
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly converts quantity from ML to G.")]
        [TestCase(0f, 11f, ExpectedResult = 0f)]
        [TestCase(11f, 0f, ExpectedResult = 0f)]
        [TestCase(11f, 11f, ExpectedResult = 121f)]
        public float Test_GetNumberOfGrams_PCS(float quantity, float gramsPerPiece)
        {
            //Arrange: Create an ingredient with the given gramsPerPiece conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerPiece: gramsPerPiece);

            //Act and Assert:
            return ingr.GetNumberOfGrams(quantity, QuantityType.PCS);
        }

        [Test, Description("Ingredient.GetNumberOfGrams correctly throws an exception if gramsPerML is null.")]
        public void Test_GetNumberOfGrams_PCS_Null()
        {
            //Arrange: Create an ingredient with the given gramsPerPiece conversion rate.
            Ingredient ingr = new Ingredient(1, "test", gramsPerPiece: null);

            //Act and Assert: Try to get the number of grams using this conversion rate and Assert that it throws an Exception.
            Assert.Throws<NullReferenceException>(delegate
            {
                ingr.GetNumberOfGrams(1f, QuantityType.PCS);
            });
        }

        [Test, Description("Two Ingredients are Equal if and only if their IDs are Equal")]
        [TestCase(1, "Orange", 1, "Orange", ExpectedResult = true)]
        [TestCase(1, "Orange", 1, "Apple", ExpectedResult = true)]
        [TestCase(1, "Orange", 2, "Orange", ExpectedResult = false)]
        [TestCase(1, "Orange", 2, "Apple", ExpectedResult = false)]
        public bool Test_Equals_Ingredient(int id1, string name1, int id2, string name2)
        {
            //Arrange: Create Ingredients with the given parameters
            Ingredient ingredient1 = new(id1, name1);
            Ingredient ingredient2 = new(id2, name2);

            //Assert: Ingredient 1 Equals Ingredient 2 if and only if id1 == id2.
            return ingredient1.Equals(ingredient2);
        }

        [Test, Description("Two Ingredients are Equal if and only if their IDs are Equal")]
        [TestCase(1, "Orange", 1, "Orange", ExpectedResult = false)]
        [TestCase(1, "Orange", 1, "Apple", ExpectedResult = false)]
        [TestCase(1, "Orange", 2, "Orange", ExpectedResult = false)]
        [TestCase(1, "Orange", 2, "Apple", ExpectedResult = false)]
        public bool Test_Equals_Object(int id1, string name1, int id2, string name2)
        {
            //Arrange: Create an Ingredient and a non-Ingredient object with the given parameters
            Ingredient ingredient1 = new(id1, name1);
            GenericObject ingredient2 = new GenericObject(id2, name2);

            //Assert: Ingredient 1 Equals Ingredient 2 if and only if id1 == id2.
            return ingredient1.Equals(ingredient2);
        }

        /// <summary>
        /// An object class that has properties ID and Name, just like Ingredient. Used to test Ingredient.Equals(object obj).
        /// </summary>
        private class GenericObject
        {
            public int ID;
            public string Name;

            public GenericObject(int id, string name)
            {
                this.ID = id;
                this.Name = name;
            }
        }
    }

    /// <summary>
    /// Test regarding the <c>IngredientList</c> object class.
    /// </summary>
    public class IngredientListTests
    {
        [Test, Description("The IngredientList constructor creates a new, empty Dictionary if no Dictionary is passed.")]
        public void Test_Constructor_ingredientsNull()
        {
            //Arrange: Create a new IngredientList using the constructor without ingredients.
            AwARe.IngredientList.Logic.IngredientList list = new("testList");

            //Assert: The List.Ingredients is an empty Dictionary.
            Assert.AreEqual(list.Ingredients, new Dictionary<Ingredient, (float, QuantityType)>());
        }

        [Test, Description("IngredientList.GetQuantity() returns the right quantity of the given ingredient.")]
        [TestCase(-100f)]
        [TestCase(0f)]
        [TestCase(100f)]
        public void Test_GetQuantity(
            float quantity)
        {
            //Arrange: Create a test Ingredient, add it to a Dictionary with the given quantity in the value,
            //and create an IngredientList with that Dictionary.
            Ingredient ingredient = new(1, "test");
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient, (quantity, QuantityType.G) }
            };
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);

            //Assert: list.GetQuantity() returns the same value as the set quantity of the ingredient.
            Assert.AreEqual(quantity, list.GetQuantity(ingredient));
        }

        [Test, Description("IngredientList.GetQuantityType() returns the right quantity type of the given ingredient.")]
        [TestCase(QuantityType.G)]
        [TestCase(QuantityType.ML)]
        [TestCase(QuantityType.PCS)]
        public void Test_GetQuantityType(
            QuantityType quantityType)
        {
            //Arrange: Create a test Ingredient, add it to a Dictionary with the given quantity type in the value,
            //and create an IngredientList with that Dictionary.
            Ingredient ingredient = new(1, "test");
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0,quantityType) }
            };
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);

            //Assert: list.GetQuantityType() returns the same value as the set QuantityType of the ingredient.
            Assert.AreEqual(quantityType, list.GetQuantityType(ingredient));
        }

        [Test, Description("IngredientList.NumberOfIngredients returns IngredientList.ingredients.Count.")]
        [TestCaseSource(nameof(testDictionaries))]
        public void Test_NumberOfIngredients(
            Dictionary<Ingredient, (float, QuantityType)> ingredients)
        {
            //Arrange: Create an IngredientList with the given ingredients Dictionary.
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);

            //Act: calculate the expected and actual number of ingredients in this list.
            int expected = ingredients.Count;
            int actual = list.NumberOfIngredients();

            //Assert: These two values are equal.
            Assert.AreEqual(expected, actual);
        }

        [Test, Description("IngredientList.ChangeName() changes the ListName of the IngredientList.")]
        [TestCase("")]
        [TestCase("test")]
        [TestCase("Test2")]
        public void Test_ChangeName(string name)
        {
            //Arrange: Create an ingredientList.
            AwARe.IngredientList.Logic.IngredientList list = new("");

            //Act: Change the ListName using ChangeName()
            list.ChangeName(name);

            //Assert: The Current ListName equals the given new name.
            Assert.AreEqual(name, list.ListName);
        }

        [Test, Description("IngredientList.AddIngredient() properly adds a new ingredient to the Dictionary.")]
        public void Test_AddIngredient()
        {
            //Arrange: Create an empty IngredientList and a new Ingredient.
            AwARe.IngredientList.Logic.IngredientList list = new("testList");
            Ingredient ingredient = new(1, "test");

            //Act: Add the Ingredient to the list using the method.
            list.AddIngredient(ingredient, 0);

            //Assert: list.Ingredients now contains the added Ingredient.
            Assert.IsTrue(list.Ingredients.ContainsKey(ingredient));
        }

        [Test, Description("IngredientList.AddIngredient() fails when trying to add an existent Ingredient.")]
        public void Test_AddIngredient_Fail()
        {
            //Arrange: Create an IngredientList with some ingredients
            //and an Ingredient with an ID identical to one of the Ingredients in the list.
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { new(1,"test1"),(0f,QuantityType.G) },
                { new(2,"test2"),(0f,QuantityType.G) }
            };
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);
            Ingredient ingredient = new(1, "test");

            //Act and Assert: list.AddIngredient() throws an exception when trying to add this ingredient.s
            Assert.Throws<ArgumentException>(delegate { list.AddIngredient(ingredient, 0); });
        }

        [Test, Description("IngredientList.RemoveIngredient() properly removes the given ingredient.")]
        public void Test_RemoveIngredient(
            [ValueSource(nameof(testDictionaries))] Dictionary<Ingredient, (float, QuantityType)> ingredients)
        {
            //Arrange: Create an ingredientList with the given ingredients Dictionary,
            //and an Ingredient with an ID that may or may not be part of the dictionary.
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);
            Ingredient ingredient = new(1, "test");

            //Act: Remove the ingredient from the list.
            list.RemoveIngredient(ingredient);

            //Assert: list.Ingredients does not contain the ingredient, nor the Ingredient that had ID == 1.
            Assert.IsFalse(list.Ingredients.ContainsKey(ingredient));
            Assert.IsFalse(list.Ingredients.ContainsKey(new(1, "test1")));
        }

        [Test, Description("IngredientList.UpdateIngredient() properly sets the dictionary quantity value to a new value.")]
        [TestCase(-1f)] // Negative quantities are currently allowed, even though they should be physically impossible.
        [TestCase(0f)] // It was also suggested to not allow or just remove an ingredient when quantity = 0.
        [TestCase(1f)]
        [TestCase(11f)]
        public void Test_UpdateIngredient_Quantity(float newQuantity)
        {
            //Arrange: Create a IngredientList containing an Ingredient (with default values).
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0f,QuantityType.G) }
            };
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);

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
        public void Test_UpdateIngredient_QuantityType(QuantityType newQuantityType)
        {
            //Arrange: Create a IngredientList containing an Ingredient (with default values).
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new()
            {
                { ingredient,(0f,QuantityType.G) }
            };
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);

            //Act: Update the value at key == ingredient and retrieve the updated quantity type.
            list.UpdateIngredient(ingredient, 0f, newQuantityType);
            QuantityType actualQuantityType = list.GetQuantityType(ingredient);

            //Assert: The quantity is set to the new value.s
            Assert.AreEqual(newQuantityType, actualQuantityType);
        }

        [Test, Description("IngredientList.UpdateIngredient() properly handles when updating an ingredient it doesn't contain.")]
        public void Test_UpdateIngredient_NotInList()
        {
            //Arrange: Create an Ingredient, an empty IngredientList, and some "new" values.
            Ingredient ingredient = new(1, "test", 1f, 1f);
            Dictionary<Ingredient, (float, QuantityType)> ingredients = new();
            AwARe.IngredientList.Logic.IngredientList list = new("testList", ingredients);
            float newQuantity = 1f;
            QuantityType newType = QuantityType.PCS;

            //Act: Update the value at key == ingredient.
            list.UpdateIngredient(ingredient, newQuantity, newType);

            //Assert: the ingredient is added to the list
            Assert.IsTrue(list.Ingredients.ContainsKey(ingredient));
        }

        [Test, Description("Tests Whether saving a list without any ingredientlists in it does not crash the program.")]
        public void Test_Save_EmptyIngredientLists_No_Crash()
        {
            //Arrange: Create a list without any ingredientlists in it.
            IngredientFileHandler ingredientFileHandler = new(new MockupIngredientDatabase());
            List<AwARe.IngredientList.Logic.IngredientList> ingredientLists = new();
            //Act + Assert: Save the empty list. Check whether it does not crash the application.
            Assert.DoesNotThrow(() => ingredientFileHandler.SaveLists(ingredientLists));
        }

        [Test, Description("Tests whether loading a List containing a valid ingredientlist return a list with an item in it.")]
        public async void Test_Read_Simple_List()
        {
            //Arrange: Construct a FileHandler with a test ingredientlists file.
            string testFile = @".\Assets\FirstParty\QA\Testing\TestAssets\IngredientList\ListWithSimpleIngredientList";
            IngredientFileHandler ingredientFileHandler = new(new MockupIngredientDatabase(), testFile);

            //Act: Read the file.
            List<AwARe.IngredientList.Logic.IngredientList> lists = await ingredientFileHandler.ReadFile();

            //Assert: The list contains a single ingredientList.
            Assert.True(lists.Count == 1);
        }

        [Test, Description("Tests whether trying to load a file that does not exist does not crash the program.")]
        public async void Test_Read_Invalid_Path_No_Crash()
        {
            //Arrange: Construct a FileHandler with an invalid path.
            IngredientFileHandler ingredientFileHandler = new(new MockupIngredientDatabase(), "Some path that does not exist");

            //Act: Read the file.
            List<AwARe.IngredientList.Logic.IngredientList> lists = await ingredientFileHandler.ReadFile();

            //Assert: The list contains a single ingredientList.
            Assert.True(lists.Count == 0);
        }
        [Test, Description("Tests whether trying to load a file that does not exist does not crash the program.")]
        public void Test_Read_Invalid_QtyType_File_Exception()
        {
            //Arrange: Construct a FileHandler with an invalid file.
            string testFile = @".\Assets\FirstParty\QA\Testing\TestAssets\IngredientList\ListWithSimpleIngredientListInvalidQtyType";
            IngredientFileHandler ingredientFileHandler = new(new MockupIngredientDatabase(), testFile);

            //Act & Assert: Reading the file throws an exeception.
            Assert.Throws<Exception>(() => ingredientFileHandler.ReadFile());
        }

        [Test, Description("Tests whether saving a loaded file does not alter the file," +
             " that is, the loaded and saved file are equal.")]
        public async void Test_Pure_Ingredientlists_Jsonstring_Conversion()
        {
            //Arrange: Construct IngredientFileHandler and read contents of the file.
            string testFile = @".\Assets\FirstParty\QA\Testing\TestAssets\IngredientList\ListWithSimpleIngredientList";
            IngredientFileHandler ingredientFileHandler = new(new MockupIngredientDatabase(), testFile);
            string testFileContent = File.ReadAllText(testFile);

            //Act: Convert contents of the file to IngredientLists, convert that back to json string.
            List<AwARe.IngredientList.Logic.IngredientList> lists = await ingredientFileHandler.ReadFile();
            string convertedLists = IngredientListsJsonHelper.IngredientListsToJSONString(lists);

            //Assert: The converted list should be the same as the original file content.
            Assert.AreEqual(testFileContent, convertedLists);
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