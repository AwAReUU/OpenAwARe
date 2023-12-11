// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// An interface for Spawner classes.<br/>
    /// A spawner class is a factory class for Instantiation instead of construction.
    /// It separates the responsibility of instantiation and initialization of component members from actual use of GameObjects.
    /// </summary>
    public interface ISpawner
    {
        /// <summary>
        /// Instantiate like <c>Object.Instantiate</c>.
        /// </summary>
        /// <returns>The instantiated instance.</returns>
        public GameObject Instantiate();

        /// <summary>
        /// Instantiate like <c>Object.Instantiate</c>.
        /// </summary>
        /// <param name="location">Placement of the object(s).</param>
        /// <param name="rotation">Rotation of the object(s).</param>
        /// <returns>The instantiated instance.</returns>
        public GameObject Instantiate(Vector3 location, Quaternion rotation);

        /// <summary>
        /// Instantiate like <c>Object.Instantiate</c>.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <returns>The instantiated instance.</returns>
        public GameObject Instantiate(Transform parent);

        /// <summary>
        /// Instantiate like <c>Object.Instantiate</c>.
        /// </summary>
        /// <param name="parent">The parent object.</param>
        /// <param name="instantiateInWorldSpace">Local or global hierarchy placement.</param>
        /// <returns>The instantiated instance.</returns>
        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace);

        /// <summary>
        /// Instantiate like <c>Object.Instantiate</c>.
        /// </summary>
        /// <param name="location">Placement of the object(s).</param>
        /// <param name="rotation">Rotation of the object(s).</param>
        /// <param name="parent">The parent object.</param>
        /// <returns>The instantiated instance.</returns>
        public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent);
    }
}