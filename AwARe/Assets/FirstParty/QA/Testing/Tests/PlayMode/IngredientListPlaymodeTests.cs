// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwARe.IngredientList.Logic;
using AwARe.IngredientList.Objects;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AwARe.Tests.IngredientList
{
    public class IngredientListPlaymodeTests
    {
        private IngredientListManager ingredientListManager;
        private readonly Ingredient mockIngredient = new(1, "name", 10, 10);

        [UnitySetUp, Description("Reset the scene before each test. Obtain the ingredientListManager")]
        public IEnumerator Setup()
        {
            yield return null; //skip one frame to ensure the scene has been loaded.
            SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/IngredientList");
            yield return null; //skip one frame to ensure the scene has been reloaded.
            ingredientListManager = GameObject.Find("IngredientListManager").GetComponent<IngredientListManager>();
        }

        [UnityTest, Description("Tests whether the default list index is set in awake.")]
        public IEnumerator Test_Default_Setup_Sets_ListIndex()
        {
            //Arrange: Scene setup (Done in Setup()).

            //Act: Skip frame to assure the Awake() method is called.
            yield return null;

            //Assert: The selection has been set (the default is 0.)
            Assert.True(ingredientListManager.IndexList == 0);
        }

        [UnityTest, Description("Tests whether searching an ingredient that does not exists does not crash the application")]
        public async IAsyncEnumerable<object> Test_Ingredient_Invalid_Search_No_Crash()
        {
            //Arrange: Create invalid ingredient name.
            const string INGREDIENT_NAME = "Invalid ingredient name";

            //Act: Search the ingredient in the database.
            List<Ingredient> foundIngredients = await ingredientListManager.SearchIngredient(INGREDIENT_NAME);
            yield return null;

            //Arrange: 
            Assert.True(foundIngredients.Count == 0);
        }

        [UnityTest, Description("Test the firing of the OnIngredientListChanged action on calling ChangeListName")]
        public IEnumerator Test_ChangeListName_Fires_OnListChange()
        {
            //Arrange: Setup event listening.
            bool eventRaised = false;
            ingredientListManager.OnIngredientListChanged += () => eventRaised = true;
            yield return null;

            //Act: Change the list.
            ingredientListManager.ChangeListName("something");

            //Assert: The list changed event was raised. 
            Assert.True(eventRaised);
        }

        [UnityTest, Description("Test the firing of the OnIngredientListChanged action on calling DeleteIngredient")]
        public IEnumerator Test_DeleteIngredient_Fires_OnListChange()
        {
            //Arrange: Setup event listening.
            bool eventRaised = false;
            ingredientListManager.OnIngredientListChanged += () => eventRaised = true;
            Ingredient ingredientToDelete = ingredientListManager.SelectedList.Ingredients.First().Key;
            yield return null;

            //Act: Change the list.
            ingredientListManager.DeleteIngredient(ingredientToDelete);

            //Assert: The list changed event was raised. 
            Assert.True(eventRaised);
        }

        //[UnityTest, Description("Test the firing of the OnIngredientListChanged action on calling UpdateIngredient")]
        //public IEnumerator Test_UpdateIngredient_Fires_OnListChange()
        //{
        //    //Arrange: Setup event listening.
        //    bool eventRaised = false;
        //    ingredientListManager.OnIngredientListChanged += () => eventRaised = true;
        //    yield return null;

        //    //Act: Change the list.
        //    //TODO: set ingredient in entry.
        //    ingredientListManager.UpdateIngredient(10, QuantityType.G);

        //    //Assert: The list changed event was raised. 
        //    Assert.True(eventRaised);
        //}

        [UnityTest, Description("Test the firing of the OnIngredientListChanged action on calling AddIngredient")]
        public IEnumerator Test_AddIngredient_Fires_OnListChange()
        {
            //Arrange: Setup event listening.
            bool eventRaised = false;
            ingredientListManager.OnIngredientListChanged += () => eventRaised = true;
            yield return null;

            //Act: Change the list.
            ingredientListManager.AddIngredient(mockIngredient, 10, QuantityType.G);

            //Assert: The list changed event was raised. 
            Assert.True(eventRaised);
        }
    }
}