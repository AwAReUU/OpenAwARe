// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwARe.MonoBehaviours;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Non-singleton class to access scene switching for buttons and such.
/// </summary>
public class SwitchSceneButton : MonoBehaviour
{
    [SerializeField] private AppScene scene;

    /// <summary>
    /// Load the scene with the given name.
    /// </summary>
    /// <param name="sceneName">The name of the scene.</param>
    public void LoadScene(string sceneName) =>
        SceneSwitcher.Get().LoadScene(sceneName);
    
    /// <summary>
    /// Load the scene with the given build index.
    /// </summary>
    /// <param name="sceneBuildIndex">The build index of the scene.</param>
    public void LoadScene(int sceneBuildIndex) =>
        SceneSwitcher.Get().LoadScene(sceneBuildIndex);
    
    /// <summary>
    /// Load the standard scene.
    /// </summary>
    /// <param name="scene">The standard scene.</param>
    public void LoadScene(AppScene scene) =>
        SceneSwitcher.Get().LoadScene(scene);

    
    /// <summary>
    /// Load the standard scene.
    /// </summary>
    public void LoadScene() =>
        LoadScene(scene);
}