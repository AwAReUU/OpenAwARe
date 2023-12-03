using System;

using UnityEngine;

namespace AwARe.Objects
{
    public class ContainerSpawner : PrefabSpawner
    {
        public Func<Transform, GameObject> getChild;

        public ContainerSpawner(GameObject prefab, GameObject child) : base(prefab)
        {
            this.getChild = (Transform parent) =>
            {
                child.transform.parent = parent;
                return child;
            };
        }

        public ContainerSpawner(GameObject prefab, ISpawner spawner) : base(prefab)
        {
            this.getChild = (Transform parent) => spawner.Instantiate(parent);
        }

        public ContainerSpawner(GameObject prefab, Func<Transform, GameObject> getChild) : base(prefab)
        {
            this.getChild = getChild;
        }

        protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
        {
            GameObject boundingBox = instantiate();
            var behaviour = boundingBox.GetComponent(typeof(Container)) as Container;
            behaviour.getChild = this.getChild;

            return boundingBox;
        }
    }
}