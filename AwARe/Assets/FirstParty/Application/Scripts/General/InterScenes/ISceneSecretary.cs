// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwARe.InterScenes
{
    /// <summary>
    /// An interface to the in between scenes stored data.
    /// </summary>
    public interface ISceneSecretary
    {
        /// <summary>
        /// Gets the set of scenes that should not be unloaded.
        /// </summary>
        /// <value>Scenes to keep alive.</value>
        public HashSet<Scene> Keepers { get; }

        /// <summary>
        /// Load the scene with the given name.
        /// </summary>
        /// <param name="sceneName">The name of the scene.</param>
        /// <param name="mode">Specify whether to keep other scenes loaded.</param>
        /// <returns>The asynchronous operation of loading.</returns>
        public YieldInstruction LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single);


        /// <summary>
        /// Load the scene with the given build index.
        /// </summary>
        /// <param name="sceneBuildIndex">The build index of the scene.</param>
        /// <param name="mode">Specify whether to keep other scenes loaded.</param>
        /// <returns>The asynchronous operation of loading.</returns>
        public YieldInstruction LoadScene(int sceneBuildIndex, LoadSceneMode mode = LoadSceneMode.Single);
    }
}