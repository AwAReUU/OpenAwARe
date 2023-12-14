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

        [OneTimeSetUp, Description("Load the test scene once.")]
        public void OneTimeSetup() => SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/AR");


        [UnitySetUp, Description("Reset the scene before each test. Obtain the objectCreationManager")]
        public IEnumerator Setup()
        {
            yield return null; //skip one frame to ensure the scene has been loaded.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
            Renderable renderable = new(model, halfExtents, 1, scale);
            List<Renderable> renderables = new() { renderable };
            renderables = Renderable.SetSurfaceRatios(renderables);
            return renderables;
        }
    }
}
