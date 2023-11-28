using System.Collections;
using System.Collections.Generic;

using AwARe.MonoBehaviours;

using NUnit.Framework;
using ObjectGeneration;
using IngredientLists;

using IngredientPipeLine;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

namespace AwARe
{
    public class ObjectGenerationTest
    {

        public ObjectCreationManager objectCreationManager;

        [OneTimeSetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Scenes/TestScene");
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest, Description("Program should not crash when trying to render an empty ingredient list.")]
        public IEnumerator EmptyIngredientListRenderingNoCrash()
        {
            //Arrange: Create empty ingredientList, and get objectCreationManager.
            objectCreationManager = GameObject.Find("ObjectCreationManager").GetComponent<ObjectCreationManager>();
            IngredientList emptyList = new(
               "Empty list",
                ingredients: new Dictionary<Ingredient, (float, QuantityType)>());
            Storage storage = Storage.Get();
            storage.ActiveIngredientList = emptyList;

            //Act: Retrieve ingredientList from storage, convert to list of renderables,
            //and place the renderables.
            objectCreationManager.OnPlaceButtonClick();
            
            //Assert: If the program reaches here, it means no exception was thrown.
            yield return null;
            Assert.Pass();
        }

        [UnityTest, Description("Do not spawn the object if it does not fit in the polygon.")]
        public IEnumerator TooBigObjectCanNotBePlaced()
        {
            //arrange
            objectCreationManager = GameObject.Find("ObjectCreationManager").GetComponent<ObjectCreationManager>();
            GameObject model = Resources.Load<GameObject>(@"Prefabs/Shapes/Cube");

            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);

            float modelSizeMultiplier = 100f;
            halfExtents *= modelSizeMultiplier;
            Renderable renderable = new(model, halfExtents, 1, modelSizeMultiplier);
            List<Renderable> renderables = new() { renderable };
            renderables = Renderable.SetSurfaceRatios(renderables);
            List<Vector3> polygonPoints = PolygonHelper.GetMockPolygon();
            GameObject[] placedObjectsBefore = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            //act
            objectCreationManager.OnPlaceButtonClick(renderables, polygonPoints);

            //assert
            yield return new WaitForSeconds(1);
            GameObject[] placedObjectsAfter = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjectsAfter == null ||
                placedObjectsBefore.Length == placedObjectsAfter.Length);
        }

        [UnityTest]
        public IEnumerator SmallObjectCanBePlaced()
        {
            //arrange
            objectCreationManager = GameObject.Find("ObjectCreationManager").GetComponent<ObjectCreationManager>();
            GameObject model = Resources.Load<GameObject>(@"Prefabs/Shapes/Cube");

            Vector3 halfExtents = PipelineManager.GetHalfExtents(model);

            float modelSizeMultiplier = 0.1f;
            halfExtents *= modelSizeMultiplier;
            Renderable renderable = new(model, halfExtents, 1, modelSizeMultiplier);
            List<Renderable> renderables = new() { renderable };
            renderables = Renderable.SetSurfaceRatios(renderables);
            List<Vector3> polygonPoints = PolygonHelper.GetMockPolygon();

            //act
            objectCreationManager.OnPlaceButtonClick(renderables, polygonPoints);

            //assert
            yield return new WaitForSeconds(1);
            GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            Assert.True(placedObjects.Length == 1);
        }
    }
}
