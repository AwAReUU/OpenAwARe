// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;

using AwARe.InterScenes.Objects;
using AwARe.Data.Objects;
using NUnit.Framework;
using UnityEngine;
using System.Linq;

using AwARe.InterScenes;

using NSubstitute;

using Unity.XR.CoreUtils;

using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

using Ingredients = AwARe.IngredientList.Logic;
using TestFixtureData = NUnit.Framework.TestFixtureData;

namespace AwARe.Testing.EditMode.General.Objects
{
    /// <summary>
    /// Test regarding the <see cref="Polygon"/> class.
    /// </summary>
    public class Polygon_Tests
    {
        public static IEnumerable TestCases_Data
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Data.Logic.Polygon()
                    );

                yield return
                    new TestCaseData(
                        new Data.Logic.Polygon(new() { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) }, 2f)
                    );
            }
        }

        [Test, Description("Getting points (Vector3) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Data))]
        public void Test_Data(Data.Logic.Polygon data)
        {
            // Arrange
            GameObject obj = new GameObject("Polygon");
            Polygon polygon = Polygon.AddComponentTo(obj, data);

            // Act
            var actual = polygon.Data;

            // Act & Assert
            Assert.AreEqual(data, actual);
        }
    }

    /// <summary>
    /// Test regarding the <see cref="Room"/> class.
    /// </summary>
    public class Room_Tests
    {
        public static IEnumerable TestCases_Data
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Data.Logic.Room()
                    );

                yield return
                    new TestCaseData(
                        new Data.Logic.Room(new(height: 0f), new() { new(height: 1f), new(height: 2f), new(height: 3f)})
                    );
            }
        }

        [Test, Description("Getting points (Vector3) of the polygon.")]
        [TestCaseSource(nameof(TestCases_Data))]
        public void Test_Data(Data.Logic.Room data)
        {
            // Arrange
            GameObject obj = new("Room");
            Room room = Room.AddComponentTo(obj, data);

            // Act
            var actual = room.Data;

            // Act & Assert
            if(data.PositivePolygon != null) Assert.AreEqual(data.PositivePolygon, actual.PositivePolygon);
            Assert.True(data.NegativePolygons.SequenceEqual(actual.NegativePolygons));
            Assert.AreEqual(data, actual);
        }
    }

    /// <summary>
    /// Test regarding the <see cref="Storage"/> class.
    /// </summary>
    public class Storage_Tests
    { 
        public static IEnumerable TestCases_Data
        {
            get
            {
                yield return
                    new TestCaseData(
                        new InterScenes.Logic.Storage {
                        ActiveIngredientList = new("testList1"),
                        ActiveRoom = new()
                        }
                    );

                yield return
                    new TestCaseData(
                        new InterScenes.Logic.Storage {
                            ActiveIngredientList = new("testList1"),
                            ActiveRoom = new()
                        }
                    );
            }
        }

        [Test, Description("Getting in between scenes kept data.")]
        [TestCaseSource(nameof(TestCases_Data))]
        public void Test_Data(InterScenes.Logic.Storage data)
        {
            // Arrange
            var storage = Storage.Get();
            storage.Data = data;

            // Act
            var actual = Storage.Get().Data;

            // Act & Assert
            Assert.AreEqual(data, actual);
        }

        public static IEnumerable TestCases_IngredientList
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Ingredients.IngredientList("testList1")
                    );

                yield return
                    new TestCaseData(
                        new Ingredients.IngredientList("testList2")
                    );
            }
        }
        
        [Test, Description("Getting the active ingredient list.")]
        [TestCaseSource(nameof(TestCases_IngredientList))]
        public void Test_IngredientList(Ingredients.IngredientList data)
        {
            // Arrange
            var storage = Storage.Get();
            storage.ActiveIngredientList = data;

            // Act
            var actual = Storage.Get().ActiveIngredientList;

            // Act & Assert
            Assert.AreEqual(data, actual);
        }

        public static IEnumerable TestCases_Room
        {
            get
            {
                yield return
                    new TestCaseData(
                        new Data.Logic.Room()
                    );

                yield return
                    new TestCaseData(
                        new Data.Logic.Room(new(height: 0f), new() { new(height: 2f) })
                    );
            }
        }

        [Test, Description("Getting the active room.")]
        [TestCaseSource(nameof(TestCases_Room))]
        public void Test_Room(Data.Logic.Room data)
        {
            // Arrange
            var storage = Storage.Get();
            storage.ActiveRoom = data;

            // Act
            var actual = Storage.Get().ActiveRoom;

            // Act & Assert
            Assert.AreEqual(data, actual);
        }
    }

    /// <summary>
    /// Test regarding the <see cref="ARSecretary"/> class.
    /// </summary>
    public class ARSecretary_Tests
    {
        private static IEnumerable TestCases_GetComponent()
        {
            yield return new TestCaseData( new Type<ARSecretary>() );
            yield return new TestCaseData( new Type<ARSession>() );
            yield return new TestCaseData( new Type<XROrigin>() );
            yield return new TestCaseData( new Type<Camera>() );
            yield return new TestCaseData( new Type<EventSystem>() );
        }

        [Test, Description("Getting in between scenes kept data.")]
        [TestCaseSource(nameof(TestCases_GetComponent))]
        public void Test_GetComponent<T>(Type<T> type)
            where T : Component
        {
            // Arrange
            var secretary = ARSecretary.Get();
            ARSecretary.SetComponent(
                new GameObject("ARSession").AddComponent<ARSession>(),
                new GameObject("ARSessionOrigin").AddComponent<XROrigin>(),
                new GameObject("ARCamera").AddComponent<Camera>(),
                new GameObject("EventSystem").AddComponent<EventSystem>()
            );

            // Act
            var actual = ARSecretary.Get().GetComponent<T>();

            // Act & Assert
            Assert.IsInstanceOf<T>(actual);
        }
    }
}