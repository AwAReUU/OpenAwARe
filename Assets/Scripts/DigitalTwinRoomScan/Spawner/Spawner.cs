using System;
using UnityEngine;
using UnityEngine.UIElements;


public interface ISpawner
{
    public GameObject Instantiate();

    public GameObject Instantiate(Vector3 location, Quaternion rotation);

    public GameObject Instantiate(Transform parent);

    public GameObject Instantiate(Transform parent, bool instantiateInWorldSpace);

    public GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent);
}

public class Spawner : ISpawner
{
    protected GameObject prefab;

    public Spawner(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public virtual GameObject Instantiate() => InstantiateHelper(() => GameObject.Instantiate(prefab));

    public virtual GameObject Instantiate(Vector3 location, Quaternion rotation) => InstantiateHelper(() => GameObject.Instantiate(prefab, location, rotation));

    public virtual GameObject Instantiate(Transform parent) => InstantiateHelper(() => GameObject.Instantiate(prefab, parent));

    public virtual GameObject Instantiate(Transform parent, bool instantiateInWorldSpace) => InstantiateHelper(() => GameObject.Instantiate(prefab, parent, instantiateInWorldSpace));

    public virtual GameObject Instantiate(Vector3 location, Quaternion rotation, Transform parent) => InstantiateHelper(() => GameObject.Instantiate(prefab, location, rotation, parent));

    protected virtual GameObject InstantiateHelper(Func<GameObject> instantiate) => instantiate();
}

public class BoundingBoxSpawner : Spawner
{
    public Func<Transform, GameObject> getChild;

    public BoundingBoxSpawner(GameObject prefab, GameObject child) : base(prefab)
    {
        this.getChild = (Transform parent) =>
        {
            child.transform.parent = parent;
            return child;
        };
    }

    public BoundingBoxSpawner(GameObject prefab, ISpawner spawner) : base(prefab)
    {
        this.getChild = (Transform parent) => spawner.Instantiate(parent);
    }

    protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
    {
        GameObject boundingBox = instantiate();
        var behaviour = boundingBox.GetComponent(typeof(BoundingBoxBehaviour)) as BoundingBoxBehaviour;
        behaviour.getChild = this.getChild;

        return boundingBox;
    }
}

public class ChunkGridSpawner<T> : Spawner
{
    ChunkGrid<T> chunkGrid;

    public ChunkGridSpawner(GameObject prefab, ChunkGrid<T> chunkGrid) : base(prefab)
    {
        this.chunkGrid = chunkGrid;
    }

    protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
    {
        // Instantiate ChunkGridObject and sets Parent
        GameObject chunkGrid = instantiate();

        // Pass ChunkGridObject a ChunkGrid
        var behaviour = chunkGrid.GetComponent(typeof(ChunkGridBehavior<T>)) as ChunkGridBehavior<T>;
        behaviour.chunkGrid = this.chunkGrid;

        return chunkGrid;
    }
}
