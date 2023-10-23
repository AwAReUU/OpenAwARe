using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BoundingBoxManager : MonoBehaviour
{
    public Vector3 location;
    public Vector3 rotation;
    public Vector3 scale;

    public GameObject boundingBox;
    public GameObject childObject;

    public GameObject sceneBox;

    //? Dit was eerst een override van een obsolete method die interacteerde 
    //? met planes en er iets creeerde, idk of dit nog nodig is maar kijk maar :)
    private void Interact()
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