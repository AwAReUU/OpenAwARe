// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using AwARe.RoomScan.Objects;
using AwARe.RoomScan.Path.Objects;

using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace AwARe.Tests.RoomScan
{
    public class PathGenTests
    {
        private RoomManager roomManager;
        private PathManager pathManager;

        [UnitySetUp, Description("Reset the scene before each test. Obtain the RoomManager")]
        public IEnumerator Setup()
        {
            for (int i = 0; i < 5; i++) { yield return null;}
            SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/Rooms");
            SceneManager.LoadScene("FirstParty/Application/Scenes/Support/GeneralSupport", LoadSceneMode.Additive);
            SceneManager.LoadScene("FirstParty/Application/Scenes/Support/ARSupport", LoadSceneMode.Additive);
            yield return null; //skip one frame to ensure the scene has been reloaded.
            roomManager = GameObject.FindObjectOfType<RoomManager>();
            pathManager = GameObject.FindObjectOfType<PathManager>();
            yield return null;
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

