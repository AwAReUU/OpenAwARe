using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using UnityEngine.UIElements;

public class BoundingBoxManager : PlaceObject
{
    public Vector3 location;
    public Vector3 rotation;
    public Vector3 scale;

    public GameObject boundingBox;
    public GameObject childObject;

    public GameObject sceneBox;

    protected override void Interact(Vector2 screenPoint)
    {
        if (sceneBox == null)
            SpawnBoxedObject(location, Quaternion.Euler(rotation), scale);
        else
            DestroyBoxedObject();
    }

    public void SpawnBoxedObject(Vector3 location, Quaternion rotation, Vector3 scale)
    {
        // Start values
        (int, int, int) gridSize = (64, 64, 64), chunkSize = (16, 16, 16);

        // Create the ChunkGrid
        ChunkGridFactory<bool> factory = new(gridSize, chunkSize);
        ChunkGrid<bool> chunkGrid = factory.Create();

        // Get spawners and factories
        var chunkGridSpawner = new ChunkGridSpawner<bool>(this.childObject, chunkGrid);
        var boundingBoxSpawner = new BoundingBoxSpawner(this.boundingBox, chunkGridSpawner);

        // Set sceneObject to manage
        this.sceneBox = boundingBoxSpawner.Instantiate(location, rotation);
        this.sceneBox.transform.localScale = scale;
    }

    public void DestroyBoxedObject()
    {
        Destroy(sceneBox);
        sceneBox = null;
    }
}

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

public class ChunkGridSpawner<Data> : Spawner
{
    ChunkGrid<Data> chunkGrid;

    public ChunkGridSpawner(GameObject prefab, ChunkGrid<Data> chunkGrid) : base(prefab)
    {
        this.chunkGrid = chunkGrid;
    }

    protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
    {
        // Instantiate ChunkGridObject and sets Parent
        GameObject chunkGrid = instantiate();

        // Pass ChunkGridObject a ChunkGrid
        var behaviour = chunkGrid.GetComponent(typeof(ChunkGridBehavior<Data>)) as ChunkGridBehavior<Data>;
        behaviour.chunkGrid = this.chunkGrid;

        return chunkGrid;
    }
}