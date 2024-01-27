// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Logic;
using AwARe.RoomScan.Objects;
using AwARe.RoomScan.Polygons.Objects;
using AwARe.Testing;
using AwARe.UI;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AwARe.Tests.PlayMode.RoomScan
{
    /// <summary>
    /// Test regarding the <see cref="PolygonDrawer"/> class.
    /// </summary>
    public class PolygonDrawer_Tests
    {
        private const string testScene = "FirstParty/QA/Testing/Tests/PlayMode/RoomScan/PolygonDrawer_Test";
        private PolygonDrawer drawer;

        [UnitySetUp, Description("Reset the scene before each test. Obtain the polygon drawer.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
            drawer = GameObject.Find("PolygonDrawer").GetComponent<PolygonDrawer>();
            yield return null;
        }

        private static IEnumerable TestCases_Drawing()
        {
            Vector3[] points;
            Data.Logic.Polygon expected;

            points = new Vector3[] { new (0, 0, 0), new (1, 0, 0), new (0, 0, 1) };
            expected = new (points.ToList());
            yield return new TestCaseData_Drawing { points = points, expected = expected };
        }

        public struct TestCaseData_Drawing
        {
            public Vector3[] points;
            public Data.Logic.Polygon expected;
        }

        [UnityTest, Description("Simulate a drawing session, check the resulting polygon.")]
        public IEnumerator Test_Drawing([ValueSource(nameof(TestCases_Drawing))] TestCaseData_Drawing testCase)
        {
            var points = testCase.points;
            var expected = testCase.expected;

            // Arrange
            IPointer pointer = Substitute.For<IPointer>();
            pointer.PointedAt.Returns(Vector3.zero);
            SubstituteReference<IPointer> pointerRef = new(pointer);
            drawer.pointer = pointerRef;
            yield return null;

            // Act - Simulates input
            drawer.StartDrawing();
            yield return null;
            foreach (var point in points)
            {
                pointer.PointedAt.Returns(point);
                drawer.AddPoint();
                yield return null;
            }
            drawer.FinishDrawing(out Data.Logic.Polygon actual);
            yield return null;

            // Assert
            Assert.AreEqual(expected, actual);
            yield return null;
        }
    }


    /// <summary>
    /// Test regarding the polygon scanning.
    /// </summary>
    public class PolygonScan_Tests
    {
        private const string appScene = "FirstParty/Application/Scenes/AppScenes/Rooms";
        private const string supportScene = "FirstParty/Application/Scenes/Support/GeneralSupport";
        private const string ARSupportScene = "FirstParty/Application/Scenes/Support/ARSupport";
        private RoomUI ui;
        private IPointer pointer;
        private RoomManager roomManager;

        [UnitySetUp, Description("Reset the scene before each test. Obtain the polygon manager.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(appScene);
            SceneManager.LoadScene(supportScene, LoadSceneMode.Additive);
            SceneManager.LoadScene(ARSupportScene, LoadSceneMode.Additive);
            yield return null;

            ui = GameObject.FindObjectOfType<RoomUI>();
            roomManager = GameObject.FindObjectOfType<RoomManager>();
            pointer = Substitute.For<IPointer>();
            pointer.PointedAt.Returns(Vector3.zero);

            var drawer = GameObject.FindObjectOfType<PolygonDrawer>();
            drawer.pointer = new SubstituteReference<IPointer>(pointer);
            yield return null;
        }

        private static IEnumerable TestCases_Scanning()
        {
            Polygon positive = new Polygon(new() { new(0, 1, 0), new(1, 1, 0), new(1, 1, 1), new(0, 1, 1) }, 3f);
            Polygon negative1 = new Polygon(new() { new(0, 2, 0), new(1, 2, 0), new(0, 2, 1) }, 4f);
            Polygon negative2 = new Polygon(new() { new(0, 3, 0), new(1, 3, 0), new(1, 3, 1) }, 5f);
            List<Polygon> negatives = new List<Polygon> { negative1, negative2 };
            List<Polygon> input = new List<Polygon> { positive, negative1, negative2 };
            yield return new TestCaseData_Scanning { inputPolygons = input, expectedPositivePolygon = positive, expectedNegativePolygons = negatives };
        }

        public struct TestCaseData_Scanning
        {
            public List<Data.Logic.Polygon> inputPolygons;
            public Data.Logic.Polygon expectedPositivePolygon;
            public List<Data.Logic.Polygon> expectedNegativePolygons;
        }

        [UnityTest, Description("Simulate a drawing session, check the resulting polygon.")]
        public IEnumerator Test_Scanning_Regular([ValueSource(nameof(TestCases_Scanning))] TestCaseData_Scanning testCase)
        {
            // Act
            foreach (var polygon in testCase.inputPolygons)
            {
                roomManager.OnCreateButtonClick();
                yield return null;
                foreach (var point in polygon.points)
                {
                    pointer.PointedAt.Returns(point);
                    roomManager.OnSelectButtonClick();
                    yield return null;
                }
                roomManager.OnConfirmButtonClick();
                yield return null;
                roomManager.OnHeightSliderChanged(polygon.height);
                yield return null;
                roomManager.OnConfirmButtonClick();
                yield return null;
                roomManager.OnConfirmButtonClick();
                yield return null;
            }

            // Assert
            var roomData = roomManager.Room.Data;
            Assert.AreEqual(testCase.expectedPositivePolygon, roomData.PositivePolygon);
            Assert.AreEqual(testCase.expectedNegativePolygons.Count, roomData.NegativePolygons.Count);
            for(var i = 0; i < testCase.expectedNegativePolygons.Count; i++)
                Assert.AreEqual(testCase.expectedNegativePolygons[i], roomData.NegativePolygons[i]);
        }
    }
}