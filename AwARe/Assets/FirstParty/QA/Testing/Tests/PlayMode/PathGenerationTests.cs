using System.Collections;

using AwARe.Data.Objects;
using AwARe.RoomScan.Objects;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Path.Objects;
using AwARe.RoomScan.Polygons.Objects;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AwARe.Testing.PlayMode
{
    public class PathGenTests
    {
        private RoomManager roomManager;
        private PathManager pathManager;

        [OneTimeSetUp, Description("Load the test scene once.")]
        public void OneTimeSetup() => SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/Rooms");

        [UnitySetUp, Description("Reset the scene before each test. Obtain the PolygonManager")]
        public IEnumerator Setup()
        {
            yield return null; //skip one frame to ensure the scene has been loaded.
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            yield return null; //skip one frame to ensure the scene has been reloaded.
            roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        }

        [UnityTest, Description("Program should not crash when generating a path with the expected input")]
        public IEnumerator Test_NoCrash_NormalInput()
        {
            //arrange
            Data.Logic.Polygon polygon = new();
            polygon.points.Add(new Vector3(1, 0, 1));
            polygon.points.Add(new Vector3(2, 0, 1));
            polygon.points.Add(new Vector3(2, 0, 2));
            polygon.points.Add(new Vector3(1, 0, 2));
            Data.Logic.Room room = new(polygon, new());
            roomManager.Room.Data = room;

            //act & assert
            Assert.DoesNotThrow(() => pathManager.StartPathGen());
            yield return null;
        }

        [UnityTest, Description("Program should not crash when trying to generate a path when given an empty polygon")]
        public IEnumerator Test_NoCrash_NoPolygon()
        {
            //arrange, act not present (as is the point of this test)
            //assert
            Assert.DoesNotThrow(() => pathManager.StartPathGen());
            yield return null;
        }


        [UnityTest, Description("Program should not crash when trying to generate a path when it has previously generated a path already")]
        public IEnumerator Test_NoCrash_PreviousPath()
        {
            //arrange
            Data.Logic.Polygon polygon = new();
            polygon.points.Add(new Vector3(1, 0, 1));
            polygon.points.Add(new Vector3(3, 0, 1));
            polygon.points.Add(new Vector3(3, 0, 3));
            polygon.points.Add(new Vector3(1, 0, 3));
            Data.Logic.Room room = new(polygon, new());
            roomManager.Room.Data = room;

            //act & assert
            Assert.DoesNotThrow(() => pathManager.StartPathGen());
            yield return null;

            //arrange
            polygon = new();
            polygon.points.Add(new Vector3(1, 0, 1));
            polygon.points.Add(new Vector3(3, 0, 1));
            polygon.points.Add(new Vector3(3, 0, 3));
            room = new(polygon, new());
            roomManager.Room.Data = room;

            //act & assert
            Assert.DoesNotThrow(() => pathManager.StartPathGen());
            yield return null;
        }
    }
}

