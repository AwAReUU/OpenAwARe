using System;

using UnityEngine;

namespace AwARe.Objects
{
    public interface ISpawner
    {
        public GameObject Instantiate();

        public GameObject Instantiate(Vector3 location, Quaternion rotation);

        public GameObject Instantiate(Transform parent);

        public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace);

        public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent);
    }

    public abstract class PrefabSpawnerTemplate : ISpawner
    {
        protected GameObject prefab;

        public virtual GameObject Instantiate() => InstantiateHelper(() => GameObject.Instantiate(prefab));

        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation) => InstantiateHelper(() => GameObject.Instantiate(prefab, location, rotation));

        public virtual GameObject Instantiate(Transform parent) => InstantiateHelper(() => GameObject.Instantiate(prefab, parent));

        public virtual GameObject Instantiate(Transform parent, bool instantiateInWorldSpace) => InstantiateHelper(() => GameObject.Instantiate(prefab, parent, instantiateInWorldSpace));

        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent) => InstantiateHelper(() => GameObject.Instantiate(prefab, location, rotation, parent));

        protected abstract GameObject InstantiateHelper(Func<GameObject> instantiate);
    }

    public class PrefabSpawner : PrefabSpawnerTemplate
    {
        public PrefabSpawner(GameObject prefab)
        {
            this.prefab = prefab;
        }

        protected override GameObject InstantiateHelper(Func<GameObject> instantiate) => instantiate();
    }

    public abstract class CompositeSpawner : ISpawner
    {
        protected ISpawner spawner;

        public virtual GameObject Instantiate() => InstantiateHelper(() => spawner.Instantiate());

        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation) => InstantiateHelper(() => spawner.Instantiate(location, rotation));

        public virtual GameObject Instantiate(Transform parent) => InstantiateHelper(() => spawner.Instantiate(parent));

        public virtual GameObject Instantiate(Transform parent, bool instantiateInWorldSpace) => InstantiateHelper(() => spawner.Instantiate(parent, instantiateInWorldSpace));

        public virtual GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent) => InstantiateHelper(() => spawner.Instantiate(location, rotation, parent));

        protected virtual GameObject InstantiateHelper(Func<GameObject> instantiate) => instantiate();
    }

    // public abstract class ObjectPoolSpawner : ISpawner (Possibly later)
}
