// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.Objects;
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
        /// Initialize this singleton component.
        /// </summary>
        /// <param name="secretary">The secretary/adapter to the Scene Manager.</param>
        public static SceneSwitcher SetComponent(SceneSecretary secretary)
        {
            SceneSwitcher switcher = Get();
            switcher.sceneSecretary = secretary;
            return switcher;
        }

        private void Awake()
        {
            // Setup singleton behaviour
            Singleton.Awake(ref instance, this);
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
        private static SceneSwitcher Instantiate()
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
    }
}