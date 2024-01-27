// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.InterScenes.Objects;
using UnityEngine;
using UnityEngine.TestTools;

namespace AwARe.UI.Objects
{
    /// <summary>
    /// Non-singleton class, attachable to buttons or managers for access to scene switching.
    /// </summary>
    [ExcludeFromCoverage]
    public class SceneSwitchHandler : MonoBehaviour
    {
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
        /// Load the previous scene.
        /// </summary>
        public void LoadLastScene() =>
            SceneSwitcher.Get().LoadLastScene();
    }
}