// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections;
using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// Class <c>ObjectDestroyer</c> Implements methods to remove all 
    /// generated objects from the scene. 
    /// (And nothing more, so no UI elements, planes, polygons etc.)
    /// </summary>
    public class ObjectDestroyer : MonoBehaviour
    {
        public void StartDestroyingObjects()
        {
            StartCoroutine(DestroyAllObjects());
        }

        /// <summary>
        /// Destroy all GameObjects in the "Placed Objects" layer.
        /// </summary>
        public IEnumerator DestroyAllObjects()
        {
            GameObject[] generatedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            if (generatedObjects == null)
                yield break;

            foreach (GameObject target in generatedObjects)
                Destroy(target);

            yield return null;
        }
    }
}
