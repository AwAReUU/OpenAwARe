// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.UI.Objects;
using NSubstitute;
using NUnit.Framework;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using Ingredients = AwARe.IngredientList.Logic;
using Plane = UnityEngine.Plane;
using Pointer = AwARe.UI.Objects.Pointer;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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

        [Test, Description("Getting the right component.")]
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

    
    /// <summary>
    /// Test regarding the <see cref="Pointer"/> class.
    /// </summary>
    public class Pointer_Tests
    {
        private static IEnumerable TestCases_SetPosition()
        {
            yield return new TestCaseData(
                new Ray(Vector3.one, Vector3.down),
                new List<Pointer.Hit> { new(4, new(1,-3,1)), new(1, new(1,0,1)), new(0.5f, new(1,0.5f,1)) },
                new Pose(Vector3.zero, Quaternion.identity),
                new Plane(Vector3.up, Vector3.zero),
                new Pose(new(1, 0.5f, 1), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(1, 0.5f, 1))
            );
            yield return new TestCaseData(
                new Ray(Vector3.one, Vector3.down),
                new List<Pointer.Hit> { new(4, new(1,-3,1)), new(1, new(1,0,1)), new(0.5f, new(1,0.5f,1)) },
                new Pose(Vector3.zero, Quaternion.identity),
                new Plane(Vector3.up, Vector3.one),
                new Pose(new(1, 0.5f, 1), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(1, 0.5f, 1))
            );
            yield return new TestCaseData(
                new Ray(new(4,3, 5), new(3,-1,2)),
                new List<Pointer.Hit> { },
                new Pose(Vector3.zero, Quaternion.identity),
                new Plane(Vector3.up, Vector3.zero),
                new Pose(new(13, 0, 11), Quaternion.identity),
                new Plane(Vector3.up, Vector3.zero)
            );
            yield return new TestCaseData(
                new Ray(Vector3.one, Vector3.down),
                new List<Pointer.Hit> { },
                new Pose(new(0.5f, 1, 2), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(0, 2, 0)),
                new Pose(new(0.5f, 1, 2), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(0, 2, 0))
            );
            yield return new TestCaseData(
                new Ray(Vector3.one, Vector3.forward),
                new List<Pointer.Hit> { },
                new Pose(new(3, 1, 2), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(0, 2, 0)),
                new Pose(new(3, 1, 2), Quaternion.identity),
                new Plane(Vector3.up, new Vector3(0, 2, 0))
            );
        }

        [Test, Description("Setting the new position of the pointer.")]
        [TestCaseSource(nameof(TestCases_SetPosition))]
        public void Test_SetPosition(Ray ray, List<Pointer.Hit> ARHits, Pose lastPose, Plane lastPlaneHit, Pose expectedPose, Plane expectedPlane)
        {
            // Arrange
            var pointer = new GameObject("Pointer").AddComponent<Pointer>();
            pointer.lastHitPlane = lastPlaneHit;
            pointer.transform.SetPositionAndRotation(lastPose.position, lastPose.rotation);

            // Act
            pointer.SetNextPosition(ray, ARHits);

            // Assert
            Assert.AreEqual(expectedPose, pointer.transform.GetWorldPose());
            Assert.AreEqual(expectedPlane, pointer.lastHitPlane);
        }
    }
}