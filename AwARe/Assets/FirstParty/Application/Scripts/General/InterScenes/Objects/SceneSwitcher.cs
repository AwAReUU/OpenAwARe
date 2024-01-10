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

using AwARe.Logic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwARe.InterScenes.Objects
{
    /// <summary>
    /// Concrete implementation of every scene loading and transitions.
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        // Singleton instance
        private static SceneSwitcher instance;
        // Scene loading handler
        private SceneSecretary sceneSecretary;

        /// <summary>
        /// Gets the set of scenes that should not be unloaded.
        /// </summary>
        /// <value>Scenes to keep alive.</value>
        public HashSet<Scene> Keepers => sceneSecretary.Keepers;

        /// <summary>
        /// Gets the look-up table from standard scenes to their file paths.
        /// Centralizes modifications to scene navigation.
        /// </summary>
        /// <value>The look-up table from standard scene to filepath.</value>
        public IReadOnlyDictionary<AppScene, string> AppSceneFilePaths { get; private set; } = new Dictionary<AppScene, string>
        {
            { AppScene.Start , ""},
            { AppScene.Home , ""},
            { AppScene.Settings , ""},
            { AppScene.AR , ""},
            { AppScene.RoomScan , ""},
            { AppScene.IngredientList , ""},
            { AppScene.Questionnaire , ""},
            { AppScene.AccountPage , ""}

        };

        /// <summary>
        /// Gets the look-up table from standard scenes to their build index.
        /// Centralizes modifications to scene navigation.
        /// Has to be constructed from the AppSceneFilePaths table at initialization.
        /// </summary>
        /// <value>The look-up table from standard scene to build index.</value>
        public IReadOnlyDictionary<AppScene, int> AppSceneBuildIndex { get; private set; }

        private void Awake()
        {
            // Setup singleton behaviour
            Singleton.Awake(ref instance, this);
            // TODO: Set build index reference to standard scenes.
            AppSceneBuildIndex = AppSceneFilePaths.ToDictionary(x => x.Key, x => SceneUtility.GetBuildIndexByScenePath(x.Value));
            // Set scene (un)loading behaviour.
            sceneSecretary = GetComponent<SceneSecretary>();
            // Keep alive between scenes
            DontDestroyOnLoad(this.gameObject);
        }

        private void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);
    
        /// <summary>
        /// Get its current instance.
        /// Instantiate a new instance if necessary.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static SceneSwitcher Get() =>
            Singleton.Get(ref instance, Instantiate);

        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static SceneSwitcher Instantiate()
        {
            GameObject me = new("SceneSwitcher");
            me.AddComponent<SceneSecretary>();
            return me.AddComponent<SceneSwitcher>();
        }
    
        /// <summary>
        /// Load the scene with the given name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="mode">Specify whether to keep other scenes loaded.</param>
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single) =>
            sceneSecretary.LoadScene(sceneName, mode);
    
        /// <summary>
        /// Load the scene with the given build index.
        /// </summary>
        /// <param name="sceneBuildIndex">The build index of the scene.</param>
        /// <param name="mode">Specify whether to keep other scenes loaded.</param>
        public void LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single) =>
            sceneSecretary.LoadScene(sceneBuildIndex, mode);
    
        /// <summary>
        /// Load the standard scene.
        /// </summary>
        /// <param name="scene">The standard scene.</param>
        /// <param name="mode">Specify whether to keep other scenes loaded.</param>
        public void LoadScene(AppScene scene, LoadSceneMode mode = LoadSceneMode.Single) =>
            LoadScene(AppSceneBuildIndex[scene], mode);
    }

    /// <summary>
    /// All standard scenes used across the application.
    /// Centralizes modifications to scene navigation.
    /// </summary>
    [Serializable]
    public enum AppScene { Start, Home, AR, RoomScan, IngredientList, Questionnaire, Settings, AccountPage }
}