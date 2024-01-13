// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections;

using AwARe.Data.Objects;
using AwARe.InterScenes.Objects;
using AwARe.NotImplemented.Objects;
using AwARe.Objects;
using AwARe.Testing;

using NSubstitute;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AwARe.Testing.PlayMode.General
{
    /// <summary>
    /// Dummy scenes for testing.
    /// </summary>
    public static class DummyScenes
    {
        private static string header = "Scene_Dummy";

        public static string Get() =>
            Get(1);

        public static string Get(int idx) =>
            header + idx;
    }

    /// <summary>
    /// Test regarding the <see cref="Liner"/> class.
    /// </summary>
    public class Liner_Tests
    {
        public Liner liner;
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/Liner_Test";

        public struct TestCase
        {
            public Vector3[] line;

            public int positionCount;
            public Vector3[] positions;
        }

        private static IEnumerable TestCases()
        {
            yield return new TestCase
            {
                line = Array.Empty<Vector3>(),
                positionCount = 0,
                positions = Array.Empty<Vector3>()
            };
            yield return new TestCase
            {
                line = new Vector3[] { new(0, 0, 0), new(1, 1, 1) },
                positionCount = 2,
                positions = new Vector3[] { new(0, 0, 0), new(1, 1, 1) }
            };
        }
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the Liner.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
            liner = GameObject.Find("Liner").GetComponent<Liner>();
        }

        [UnityTest, Description("Update Lines")]
        public IEnumerator Test_UpdateLines([ValueSource(nameof(TestCases))] TestCase testCase)
        {
            // Arrange
            var logic = Substitute.For<ILinerLogic>();
            logic.Line.Returns(testCase.line);
            liner.logic = new SubstituteReference<ILinerLogic>(logic);

            // Act
            liner.UpdateLine();
            for (var frame = 0; frame < 5; frame++)
                yield return null;

            // Assert
            int positionCount = liner.lineRenderer.positionCount;
            Assert.AreEqual(testCase.positionCount, positionCount);
            var positions = new Vector3[positionCount];
            liner.lineRenderer.GetPositions(positions);
            Assert.AreEqual(testCase.positions, positions);

            yield return null;
        }
    }

    /// <summary>
    /// Test regarding the <see cref="RoomLiner"/> class.
    /// </summary>
    public class RoomLiner_Tests
    {
        public RoomLiner roomLiner;
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/RoomLiner_Test";
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the RoomLiner.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
            roomLiner = GameObject.Find("Room").GetComponent<RoomLiner>();
            yield return null;
        }

        [UnityTest, Description("Update Lines")]
        public IEnumerator Test_UpdateLines()
        {
            // Arrange
            var logic = Substitute.For<ILinerLogic>();
            logic.Line.Returns( new[]{ Vector3.zero } );
            var substitute = new SubstituteReference<ILinerLogic>(logic);
            roomLiner.positivePolygonLiner.logic = substitute;
            foreach(var liner in roomLiner.negativePolygonLiners)
                liner.logic = substitute;

            // Act
            roomLiner.UpdateLines();
            for (var frame = 0; frame < 10; frame++)
                yield return null;

            // Assert
            Assert.AreEqual( 1, roomLiner.positivePolygonLiner.lineRenderer.positionCount);
            foreach (var liner in roomLiner.negativePolygonLiners)
                Assert.AreEqual( 1, liner.lineRenderer.positionCount);

            yield return null;
        }
    }

    /// <summary>
    /// Test regarding the <see cref="Mesher"/> class.
    /// </summary>
    public class Mesher_Tests
    {
        public Mesher mesher;
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/Mesher_Test";

        public struct TestCase
        {
            public Mesh mesh;
        }

        private static IEnumerable TestCases()
        {
            yield return new TestCase
            {
                mesh = new()
            };
            yield return new TestCase
            {
                mesh = new()
                {
                    vertices = new Vector3[] { new(0, 0, 0), new(1, 0, 0), new(0, 0, 1) },
                    triangles = new[] { 0, 1, 2 }
                }
            };
        }
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the Mesher.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
            mesher = GameObject.Find("Mesher").GetComponent<Mesher>();
        }

        [UnityTest, Description("Update Mesh")]
        public IEnumerator Test_UpdateMesh([ValueSource(nameof(TestCases))] TestCase testCase)
        {
            // Arrange
            var logic = Substitute.For<IMesherLogic>();
            logic.Mesh.Returns(testCase.mesh);
            mesher.logic = new SubstituteReference<IMesherLogic>(logic);

            // Act
            mesher.UpdateMesh();
            for (var frame = 0; frame < 5; frame++)
                yield return null;

            // Assert
            Assert.AreEqual(testCase.mesh.vertices, mesher.meshFilter.mesh.vertices);
            Assert.AreEqual(testCase.mesh.triangles, mesher.meshFilter.mesh.triangles);
            yield return null;
        }
    }
    
    /// <summary>
    /// Test regarding the <see cref="SceneSwitcher"/> class.
    /// </summary>
    public class SceneSwitcher_Tests
    {
        public SceneSwitcher switcher;
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/SceneSwitcher_Test";
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the SceneSwitcher.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
            switcher = GameObject.Find("SceneSwitcher").GetComponent<SceneSwitcher>();
        }

        [UnityTest, Description("Test switching scenes by name.")]
        public IEnumerator Test_LoadSceneByName()
        {
            // Arrange
            SceneManager.LoadScene(DummyScenes.Get(1));
            SceneManager.LoadScene(DummyScenes.Get(2), LoadSceneMode.Additive);
            for (var frame = 0; frame < 5; frame++)
                yield return null;
            
            Scene kept = SceneManager.GetSceneByName(DummyScenes.Get(1));
            Scene unloaded = SceneManager.GetSceneByName(DummyScenes.Get(2));
            var switcher = SceneSwitcher.Get();
            switcher.Keepers.Add(kept);

            // Act
            switcher.LoadScene(DummyScenes.Get(3));
            for (var frame = 0; frame < 5; frame++)
                yield return null;
            Scene @new = SceneManager.GetSceneByName(DummyScenes.Get(3));

            // Assert
            Assert.True(kept.isLoaded);
            Assert.False(unloaded.isLoaded);
            Assert.True(@new.isLoaded);
            yield return null;
        }
    }

    /// <summary>
    /// Test regarding the <see cref="StartLoader"/> class.
    /// </summary>
    public class StartLoader_Tests
    {
        private string startScene = "FirstParty/QA/Testing/Tests/PlayMode/General/Scene_Dummies/Empty_Scene";
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/StartLoader_Test";
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the SceneSwitcher.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(startScene);
            yield return null;
        }

        [UnityTest, Description("Loading initial scenes.")]
        public IEnumerator Test_LoadSceneByName()
        {
            // Act
            SceneManager.LoadScene(testScene);
            for (var frame = 0; frame < 5; frame++)
                yield return null;

            // Assert
            Scene active = SceneManager.GetSceneByName(DummyScenes.Get(1));
            Scene support1 = SceneManager.GetSceneByName(DummyScenes.Get(2));
            Scene support2 = SceneManager.GetSceneByName(DummyScenes.Get(3));

            Assert.True(active.isLoaded);
            Assert.True(support1.isLoaded);
            Assert.True(support2.isLoaded);
            yield return null;
        }
    }

    
    /// <summary>
    /// Test regarding the <see cref="NotImplementedManager"/> class.
    /// </summary>
    public class NotImplementedManager_Tests
    {
        private string testScene = "FirstParty/QA/Testing/Tests/PlayMode/General/NotImplementedManager_Test";
        
        [UnitySetUp, Description("Reset the scene before each test. Obtain the NotImplementedManager.")]
        public IEnumerator Setup()
        {
            SceneManager.LoadScene(testScene);
            yield return null;
        }
        
        [UnityTest, Description("Show the not-implemented-warning pop up.")]
        public IEnumerator Test_ShowPopUp()
        {
            // Act
            GameObject popUp = NotImplementedManager.Get().ShowPopUp();
            yield return null;

            // Assert
            Assert.True(popUp.activeSelf);
            yield return null;
        }
        
        [UnityTest, Description("Hide the not-implemented-warning pop up.")]
        public IEnumerator Test_HidePopUp()
        {
            // Arrange
            GameObject popUp = NotImplementedManager.Get().ShowPopUp();
            yield return null;

            // Act
            NotImplementedManager.Get().HidePopUp();
            yield return null;

            // Assert
            Assert.True(popUp == null);
            yield return null;
        }
    }
}