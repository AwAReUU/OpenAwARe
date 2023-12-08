using System.Collections.Generic;

using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// Class <c>ObjectObtainer</c> is used to find all gameObjects in a given layer.
    /// </summary>
    public class ObjectObtainer : MonoBehaviour
    {
        /// <summary>
        /// Obtain all Gameobjects in <paramref name="layer"/>.
        /// Overload method: both string and int are accepted as parameter.
        /// </summary>
        /// <param><c>layer</c> is the layer to find all objects from</param>
        /// <returns>All GameObjects in that layer.</returns>
        public static GameObject[] FindGameObjectsInLayer(int layer)
        {
            var goArray = FindObjectsOfType(typeof(GameObject)) as GameObject[];
            var goList = new List<GameObject>();
            for (int i = 0; i < goArray.Length; i++)
                if (goArray[i].layer == layer)
                    goList.Add(goArray[i]);

            if (goList.Count == 0)
                return null;
            return goList.ToArray();
        }

        /// <summary>
        /// Obtain all Gameobjects in <paramref name="stringLayer"/>.
        /// Overload method: both string and int are accepted as parameter.
        /// </summary>
        /// <param><c>layer</c> is the layer to find all objects from</param>
        /// <returns>All GameObjects in that layer.</returns>
        public static GameObject[] FindGameObjectsInLayer(string stringLayer) => 
            FindGameObjectsInLayer(LayerMask.NameToLayer(stringLayer));
    }
}