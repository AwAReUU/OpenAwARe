// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using UnityEngine;

namespace AwARe.Objects
{
    /// <summary>
    /// The Spawner for the Container Prefab.
    /// The spawner passes the getter to the GameObject to contain.
    /// </summary>
    public class ContainerSpawner : PrefabSpawner
    {
        /// <summary>
        /// A Getter Callback, providing the GameObject to contain.
        /// </summary>
        public Func<Transform, GameObject> getChild;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerSpawner"/> class.
        /// </summary>
        /// <param name="prefab">The specific Container prefab.</param>
        /// <param name="child">The GameObject to parent.</param>
        public ContainerSpawner(GameObject prefab, GameObject child) : base(prefab)
        {
            this.getChild = (Transform parent) =>
            {
                child.transform.parent = parent;
                return child;
            };
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerSpawner"/> class.
        /// </summary>
        /// <param name="prefab">The specific Container prefab.</param>
        /// <param name="spawner">The Spawner of the GameObject to parent.</param>
        public ContainerSpawner(GameObject prefab, ISpawner spawner) : base(prefab)
        {
            this.getChild = spawner.Instantiate;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerSpawner"/> class.
        /// </summary>
        /// <param name="prefab">The specific Container prefab.</param>
        /// <param name="getChild">The getter callback of the GameObject to parent.</param>
        public ContainerSpawner(GameObject prefab, Func<Transform, GameObject> getChild) : base(prefab)
        {
            this.getChild = getChild;
        }

        /// <inheritdoc/>
        protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
        {
            GameObject boundingBox = instantiate();
            var behaviour = boundingBox.GetComponent(typeof(Container)) as Container;
            behaviour.getChild = this.getChild;

            return boundingBox;
        }
    }
}