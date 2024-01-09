// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace AwARe.InterScenes.Objects
{
    /// <summary>
    /// Loads and activates initial scenes and supports scenes.
    /// Unity only activates.
    /// </summary>
    public class StartLoader : MonoBehaviour
    {
        // Initial scenes to load.
        [SerializeField] private string firstScene;
        [SerializeField] private List<string> supportScenes;
        
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake() =>
            StartCoroutine(LoadScenes());

        /// <summary>
        /// Loads and activates the starting scene(s).
        /// </summary>
        /// <returns>An IEnumerator for its coroutine.</returns>
        private IEnumerator LoadScenes()
        {
            // Perform all loads.
            AsyncOperation first = SceneManager.LoadSceneAsync(firstScene, LoadSceneMode.Additive);
            List<AsyncOperation> operations = new();
            foreach (var scene in supportScenes)
                operations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));

            // Activate first scene.
            yield return first;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(firstScene));

            // Wait till all loads finish.
            foreach (var operation in operations)
                yield return operation;

            // Destroy script to prevent repeats.
            Destroy(this);
        }
    }
}