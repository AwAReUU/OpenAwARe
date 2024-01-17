// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AwARe.Objects
{
    /// <summary>
    /// Template for a single prefab spawner.
    /// </summary>
    public abstract class PrefabSpawnerTemplate : ISpawner
    {
        /// <summary>
        /// Prefab to be cloned by Spawner.
        /// </summary>
        protected GameObject prefab;

        /// <inheritdoc/>
        public virtual GameObject Instantiate() => InstantiateHelper(() => Object.Instantiate(prefab));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation) => InstantiateHelper(() => Object.Instantiate(prefab, location, rotation));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Transform parent) => InstantiateHelper(() => Object.Instantiate(prefab, parent));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Transform parent, bool instantiateInWorldSpace) => InstantiateHelper(() => Object.Instantiate(prefab, parent, instantiateInWorldSpace));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent) => InstantiateHelper(() => Object.Instantiate(prefab, location, rotation, parent));

        /// <summary>
        /// Implements the actual instantiation behaviour, preparation and initialization.
        /// </summary>
        /// <param name="instantiate">Instantiation callback.</param>
        /// <returns>The instantiated and initialized instance.</returns>
        protected abstract GameObject InstantiateHelper(Func<GameObject> instantiate);
    }

    /// <summary>
    /// Template for a single prefab spawner.
    /// </summary>
    public class PrefabSpawner : PrefabSpawnerTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabSpawner"/> class.
        /// </summary>
        /// <param name="prefab">The prefab to clone.</param>
        public PrefabSpawner(GameObject prefab)
        {
            this.prefab = prefab;
        }
        
        /// <inheritdoc/>
        protected override GameObject InstantiateHelper(Func<GameObject> instantiate) => instantiate();
    }

    /// <summary>
    /// Simple spawner for a Composite pattern.
    /// </summary>
    public abstract class CompositeSpawner : ISpawner
    {
        /// <summary>
        /// The element Spawner.
        /// </summary>
        protected ISpawner spawner;

        /// <inheritdoc/>
        public virtual GameObject Instantiate() => InstantiateHelper(() => spawner.Instantiate());

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation) => InstantiateHelper(() => spawner.Instantiate(location, rotation));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Transform parent) => InstantiateHelper(() => spawner.Instantiate(parent));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Transform parent, bool instantiateInWorldSpace) => InstantiateHelper(() => spawner.Instantiate(parent, instantiateInWorldSpace));

        /// <inheritdoc/>
        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent) => InstantiateHelper(() => spawner.Instantiate(location, rotation, parent));
        
        /// <summary>
        /// Implements the actual instantiation behaviour, preparation and initialization.
        /// </summary>
        /// <param name="instantiate">Instantiation callback.</param>
        /// <returns>The instantiated and initialized instance.</returns>
        protected virtual GameObject InstantiateHelper(Func<GameObject> instantiate) => instantiate();
    }

    // public abstract class ObjectPoolSpawner : ISpawner (Possibly later)
}
