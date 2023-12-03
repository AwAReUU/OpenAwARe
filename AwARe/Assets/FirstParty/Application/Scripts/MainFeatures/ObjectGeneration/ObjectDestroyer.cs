using System.Collections.Generic;

using AwARe.ObjectGeneration;

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
