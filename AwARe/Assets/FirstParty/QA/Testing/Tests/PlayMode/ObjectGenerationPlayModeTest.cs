using System.Collections;
using System.Collections.Generic;

using IL = AwARe.IngredientList.Logic;
using AwARe.IngredientList.Logic;
using AwARe.InterScenes.Objects;
using AwARe.ObjectGeneration;
using AwARe.ResourcePipeline.Objects;
using AwARe.RoomScan.Polygons.Logic;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace AwARe
{
    /// <summary>
    /// Class <c>ObjectGenerationTest</c> Contains some playGroundTests to ensure proper object generation.
    /// </summary>
    public class ObjectGenerationPlayModeTest
    {
        private ObjectCreationManager objectCreationManager;

        [UnitySetUp, Description("Reset the scene before each test. Obtain the objectCreationManager")]
        public IEnumerator Setup()
        {
            yield return null; //skip one frame to ensure the scene has been loaded.
            SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/AR");
            yield return null; //skip one frame to ensure the scene has been reloaded.
            objectCreationManager = GameObject.Find("ObjectCreationManager").GetComponent<ObjectCreationManager>();
        }

        [UnityTest, Description("Program should not crash when trying to render an empty ingredient list.")]
        public IEnumerator EmptyIngredientListRenderingNoCrash()
        {
            //Arrange: Create empty ingredientList, and get objectCreationManager.
            IL.IngredientList emptyList = new(
               "Empty list",
                ingredients: new Dictionary<Ingredient, (float, QuantityType)>());
            Storage storage = Storage.Get();
            storage.ActiveIngredientList = emptyList;

            //Act & Assert: Retrieve ingredientList from storage, convert to list of renderables,
            //and place the renderables.
            Assert.DoesNotThrow(() => objectCreationManager.OnPlaceButtonClick());

            yield return null;
        }

        [UnityTest, Description("Do not spawn the object if it does not fit in the polygon.")]
        public IEnumerator TooBigObjectCanNotBePlaced()
        {
            //arrange: Create a polygon and a big Renderable.
            List<Renderable> renderables = GetSingleRenderable(100f);

            //act: Try place Renderable in the polygon.
            objectCreationManager.PlaceRenderables(renderables, new TestRoom(), new Mesh());

            //assert: The amount of placed objects did not change.
            //yield return new WaitForSeconds(1);
            yield return null;
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects == null);
        }

        [UnityTest, Description("Check whether object placement works for an object that is guaranteed to fit.")]
        public IEnumerator SmallObjectCanBePlaced()
        {
            //arrange: Create a polygon and a small Renderable.
            List<Renderable> renderables = GetSingleRenderable(0.1f);

            //act: Place the renderable
            objectCreationManager.PlaceRenderables(renderables, new TestRoom(), new Mesh());

            //assert: There is one Renderable in the scene.
            yield return null;
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects.Length == 1);
        }

        [UnityTest, Description("Makes sure that the ObjectObtainer works")]
        public IEnumerator ObjectObtainerWorks()
        {
            //Arrange: Create a gameObject in a layer.
            string layer = "Placed Objects";
            GameObject[] obtainedObjectsBefore = ObjectObtainer.FindGameObjectsInLayer(layer);
            GameObject _ = new GameObject("TestObject") { layer = LayerMask.NameToLayer(layer) };
            yield return null;

            //Act: Count the amount of objects in the layer.
            GameObject[] obtainedObjectsAfter = ObjectObtainer.FindGameObjectsInLayer(layer);

            //Assert: The amount of gameObjects in the layer changed after creating the object.
            Assert.True(obtainedObjectsBefore == null && obtainedObjectsAfter.Length == 1);
        }

        [UnityTest, Description("Makes sure that the ObjectDestroyer works")]
        public IEnumerator ObjectDestroyerWorks()
        {
            //Arrange: Create an empty gameObject in a layer.
            string layer = "Placed Objects";
            GameObject _ = new GameObject { layer = LayerMask.NameToLayer(layer) };
            GameObject[] obtainedObjectsBefore = ObjectObtainer.FindGameObjectsInLayer(layer);

            //Act: Destroy the object, and obtain the objects in the previously mentioned layer.
            new GameObject().AddComponent<ObjectDestroyer>().DestroyAllObjects();

            yield return null;
            GameObject[] obtainedObjectsAfter = ObjectObtainer.FindGameObjectsInLayer(layer);

            //Assert: The Amount of gameObjects in the layer changed from 1 to 0.
            Assert.True(obtainedObjectsBefore.Length == 1 && obtainedObjectsAfter == null);
        }


        [UnityTest, Description("Check whether object placement works for multiple objects that are guaranteed to fit.")]
        public IEnumerator TestMultipleSmallObjects()
        {
            //arrange: Create a polygon and 4 small Renderables.
            List<Renderable> renderables = GetMultipleRenderables(4, 0.1f);

            //act: Place the renderables
            objectCreationManager.PlaceRenderables(renderables, new TestRoom(), new Mesh());

            //assert: There are 4 Renderables in the scene.
            yield return null;
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects.Length == 4);
        }

        [UnityTest, Description("Check whether object stacking works")]
        public IEnumerator TestObjectStackingNormal()
        {
            //arrange: Create a polygon and a lot of renderables
            List<Renderable> renderables = GetMultipleRenderables(1000, 10f);

            //act: Place the renderable
            objectCreationManager.PlaceRenderables(renderables, new TestRoom(), new Mesh());

            //assert: All renderables are in the scene and there is a stack
            yield return null;
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects.Length == 1000); //ensure all objects have been placed

            bool atleastonestacked = false;
            for(int i = 0; i < renderables.Count; i++)
            {
                if(renderables[i].objStacks.Count > 0)
                {
                    atleastonestacked = true;
                    break;
                }
            }
            Assert.True(atleastonestacked);
        }

        [UnityTest, Description("Check whether an empty renderable list behaves correctly")]
        public IEnumerator TestEmptyList()
        {
            //arrange: Create a polygon and a small Renderable.
            List<Renderable> renderables = new();

            //act: Place the renderable
            objectCreationManager.PlaceRenderables(renderables, new TestRoom(), new Mesh());

            //assert: No objects have been placed
            yield return null;
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects == null); //ensure all objects have been placed
        }


        //-----------------------------------------------------------------------------------------------------




        [UnityTest, Description("Makes sure the renderables are placed seperately if necessary")]
        public IEnumerator PlaceRenderablesInSeparateRooms()
        {
            // Arrange: Fill storage with necessary ingredient list & room 
            Storage storage = Storage.Get();

            Ingredient IngredientPlant = new Ingredient( 7,     "Grape",  null,    8);  // grape
            Ingredient IngredientAnimal = new Ingredient(13,   "Chicken",  null,  250); // chicken
            Ingredient IngredientWater = new Ingredient( 1,     "Water",  1.0f, null);  // water

            IL.IngredientList ingredientList = new(
               "IngredientList",
                ingredients: new Dictionary<Ingredient, (float, QuantityType)>());
            ingredientList.AddIngredient(IngredientAnimal, 1);
            ingredientList.AddIngredient(IngredientPlant, 1);
            ingredientList.AddIngredient(IngredientWater, 1);
            
            storage.ActiveIngredientList = ingredientList;
            storage.ActiveRoom = new TestRoom();

            yield return null;

            // Act: Place all the renderables in the storage in the room 
            objectCreationManager.OnPlaceButtonClick();
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            
            // Assert: Check if the correct renderables are placed in each room.
            yield return null;


        }

        [UnityTest, Description("Makes sure that area of the renderables are computed correctly")]
        public IEnumerator ComputeRenderableSpaceNeededWorks()
        { 
            //Arrange: Create renderable lists.
            List<Renderable> list0 = new (); // empty 
            List<Renderable> list1 = new (); // renderables with quantity = 1
            List<Renderable> list2 = new (); // renderables with quantity > 1

            //Act: Fill the renderable lists.
            GameObject model = Resources.Load<GameObject>(@"Models/Shapes/Cube");
            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);
            float scale = 1;
            list1.Add(new Renderable(model, halfExtents, 1, scale, ResourcePipeline.Logic.ResourceType.Water));
            list2.Add(new Renderable(model, halfExtents, 5, scale, ResourcePipeline.Logic.ResourceType.Water));
            float list0spaceNeeded = objectCreationManager.ComputeRenderableSpaceNeeded(list0);
            float list1spaceNeeded = objectCreationManager.ComputeRenderableSpaceNeeded(list1);
            float list2spaceNeeded = objectCreationManager.ComputeRenderableSpaceNeeded(list2);
            Debug.Log("halfext: " + halfExtents);
            Debug.Log(list1spaceNeeded);

            //Assert: The area for each renderable list should be within the allowed range 
            Assert.True(
            list0spaceNeeded == 0 
            && list1spaceNeeded > 0.009f && list1spaceNeeded < 0.011f
            && list2spaceNeeded > 0.049f && list2spaceNeeded < 0.051f);
            yield return null;
        }






        /// <summary>
        /// Create a singleton list containing a simple renderable object.
        /// </summary>
        /// <param name="scale">Size multiplier of the object.</param>
        /// <returns>Singleton list containing a renderable Cube.</returns>
        private List<Renderable> GetSingleRenderable(float scale)
        {
            GameObject model = Resources.Load<GameObject>(@"Models/Shapes/Cube");

            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);
            halfExtents *= scale;
            Renderable renderable = new(model, halfExtents, 1, scale, ResourcePipeline.Logic.ResourceType.Water);
            List<Renderable> renderables = new() { renderable };
            renderables = Renderable.SetSurfaceRatios(renderables);
            return renderables;
        }

        private List<Renderable> GetMultipleRenderables(int numberofrenderables, float scale)
        {
            GameObject model = Resources.Load<GameObject>(@"Models/Shapes/Cube");

            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);
            halfExtents *= scale;
            Renderable renderable = new(model, halfExtents, numberofrenderables, scale, ResourcePipeline.Logic.ResourceType.Water);
            List<Renderable> renderables = new() { renderable };
            renderables = Renderable.SetSurfaceRatios(renderables);
            return renderables;
        }

        private List<Renderable> GetMixedRenderables(float scale)
        {
            GameObject model = Resources.Load<GameObject>(@"Models/Shapes/Cube");

            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);
            halfExtents *= scale;
            Renderable renderableWater = new(model, halfExtents, 1, scale, ResourcePipeline.Logic.ResourceType.Water);
            Renderable renderableAnimal = new(model, halfExtents, 1, scale, ResourcePipeline.Logic.ResourceType.Animal); 
            Renderable renderablePlant = new(model, halfExtents, 1, scale, ResourcePipeline.Logic.ResourceType.Plant);

            List<Renderable> renderables = new() { renderableWater, renderableAnimal, renderablePlant };
            renderables = Renderable.SetSurfaceRatios(renderables);
            return renderables;
        }
    }
}
