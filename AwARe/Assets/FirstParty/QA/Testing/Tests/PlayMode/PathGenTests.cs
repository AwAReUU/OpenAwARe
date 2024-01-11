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


    [UnitySetUp, Description("Reset the scene before each test. Obtain the objectCreationManager")]
    public IEnumerator Setup()
    {
        yield return null; //skip one frame to ensure the scene has been loaded.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null; //skip one frame to ensure the scene has been reloaded.
        polygonManager = GameObject.Find("PolygonManager").GetComponent<PolygonManager>();
        visualizer = GameObject.Find("PolygonManager").transform.Find("PathVisualiser").GetComponent<PathVisualizer>();
    }

    [UnityTest, Description("Program should not crash when generating a path")]
    public IEnumerator Test_NoCrash()
    {
        Polygon polygon = new();
        polygon.AddPoint(new Vector3(1, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 2));
        polygon.AddPoint(new Vector3(1, 0, 2));
        polygonManager.SetCurrentPolygon(polygon);

        polygonManager.OnApplyButtonClick();
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());


        yield return null;
    }

    [UnityTest, Description("Program should not crash when trying to generate a path when given an empty polygon")]
    public IEnumerator Test_NoCrash_NoPolygon()
    {
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;
    }


    [UnityTest, Description("Program should not crash when trying to generate a path when it has previously generated a path already")]
    public IEnumerator Test_NoCrash_PreviousPath()
    {
        Polygon polygon = new();
        polygon.AddPoint(new Vector3(1, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 1));
        polygon.AddPoint(new Vector3(2, 0, 2));
        polygon.AddPoint(new Vector3(1, 0, 2));
        polygonManager.SetCurrentPolygon(polygon);

        polygonManager.OnApplyButtonClick();
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;

        polygonManager.OnResetButtonClick();
        polygonManager.OnApplyButtonClick();
        Assert.DoesNotThrow(() => polygonManager.OnPathButtonClick());
        yield return null;
    }
}

