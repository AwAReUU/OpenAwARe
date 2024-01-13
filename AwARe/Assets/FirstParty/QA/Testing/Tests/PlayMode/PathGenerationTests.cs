using System.Collections;
using System.Collections.Generic;
using AwARe.Data.Logic;
using AwARe.RoomScan.Path;
using AwARe.RoomScan.Polygons.Objects;
using NUnit.Framework;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PathGenTests
{
    private PathVisualizer visualizer;
    private PolygonManager polygonManager;

    [OneTimeSetUp, Description("Load the test scene once.")]
    public void OneTimeSetup() => SceneManager.LoadScene("FirstParty/Application/Scenes/AppScenes/RoomScan");

    [UnitySetUp, Description("Reset the scene before each test. Obtain the PolygonManager")]
    public IEnumerator Setup()
    {
        yield return null; //skip one frame to ensure the scene has been loaded.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null; //skip one frame to ensure the scene has been reloaded.
        polygonManager = GameObject.Find("PolygonManager").GetComponent<PolygonManager>();
        visualizer = GameObject.Find("PolygonManager").transform.Find("PathVisualiser").GetComponent<PathVisualizer>();
    }

    [UnityTest, Description("Program should not crash when generating a path with the expected input")]
    public IEnumerator Test_NoCrash_NormalInput()
    {
        //arrange
        Polygon polygon = new();
        polygon.AddPoint(new Vector3(1, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 2));
        polygon.AddPoint(new Vector3(1, 0, 2));
        polygonManager.SetCurrentPolygon(polygon);

        //act
        polygonManager.OnApplyButtonClick();

        //assert
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());

        yield return null;
    }

    [UnityTest, Description("Program should not crash when trying to generate a path when given an empty polygon")]
    public IEnumerator Test_NoCrash_NoPolygon()
    {
        //arrange, act not present (as is the point of this test)
        //assert
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;
    }


    [UnityTest, Description("Program should not crash when trying to generate a path when it has previously generated a path already")]
    public IEnumerator Test_NoCrash_PreviousPath()
    {
        //arrange
        Polygon polygon = new();
        polygon.AddPoint(new Vector3(1, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 2));
        polygon.AddPoint(new Vector3(1, 0, 2));
        polygonManager.SetCurrentPolygon(polygon);

        //acte
        polygonManager.OnApplyButtonClick();
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;
        polygonManager.OnResetButtonClick();
        polygonManager.OnApplyButtonClick();
        
        //assert
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;
    }
}

