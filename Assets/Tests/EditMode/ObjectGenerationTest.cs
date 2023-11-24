using System.Collections;
using System.Collections.Generic;

using AwARe.MonoBehaviours;

using IngredientLists;

using NUnit.Framework;

using ObjectGeneration;

using UnityEngine;
using UnityEngine.TestTools;

namespace AwARe
{
    public class ObjectGenerationTest
    {
        // A Test behaves as an ordinary method
        [Test, Description("Program should not crash when trying to render an empty ingredient list.")]
        public void EmptyIngredientListRenderingNoException()
        {
            //Arrange
            IngredientList emptyList = new(
                "Empty list",
                ingredients: new Dictionary<Ingredient, (float, QuantityType)>());
            GameObject gameObject = new GameObject();
            ObjectCreationManager objectCreationManager = gameObject.AddComponent<ObjectCreationManager>();
            Storage storage = Storage.Get();
            storage.ActiveIngredientList = emptyList;

            //Act
            objectCreationManager.OnPlaceButtonClick();
            
            //Assert
            Assert.Pass(); //If exception is thrown, program will not reach here.
        }
    }
}
