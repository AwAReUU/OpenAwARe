// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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
        /// <summary>
        /// Destroy all GameObjects in the "Placed Objects" layer
        /// </summary>
        public void DestroyAllObjects()
        {
            GameObject[] generatedObjects = ObjectObtainer.FindGameObjectsInLayer("Placed Objects");
            if (generatedObjects == null)
                return;

            foreach (GameObject target in generatedObjects)
                Destroy(target);
        }
    }
}
