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
        private ObjectCreationManager prefab;

        //[Test]
        //public void OnlyPlaceInBounds()
        //{
        //    //Arrange: 
        //    GameObject prefab = Resources.Load<GameObject>(path: "Prefabs/Shapes/Cube");
        //    Vector3 halfExtents = new Vector3(100,100,100);
        //    Renderable renderable = new Renderable(prefab, halfExtents,1,100);
        //    ObjectPlacer objectPlacer = new ObjectPlacer();
        //    List<Vector3> polygonPoints = new List<Vector3>() {
        //        new Vector3(-8f, -0.87f, -8f),
        //        new Vector3(-8f, -0.87f, -3f),
        //        new Vector3(-3f, -0.87f, -3f),
        //        new Vector3(-3f, -0.87f, -8f)
        //    };
        //    List<Renderable> renderables = new List<Renderable> { renderable };

        //    //Act: place the oversized object in polygon
        //    objectPlacer.PlaceRenderables(renderables, polygonPoints);
            
        //    //Assert: 
        //    GameObject[] placedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");

        //    Assert.True(placedObjects.Length == 0);
        //}
    }
}
