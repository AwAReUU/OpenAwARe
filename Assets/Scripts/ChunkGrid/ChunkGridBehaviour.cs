using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGridBehavior<Data> : MonoBehaviour, IChunkGridSize
{
    public (int, int, int) gridSize, chunkSize;
    public GameObject chunkObject;

    public IChunkGrid<Data> chunkGrid;
    public GameObject[,,] chunkObjects;

    public (int, int, int) GridSize => chunkGrid.GridSize;

    public (int, int, int) NroChunks => chunkGrid.NroChunks;

    public (int, int, int) ChunkSize => chunkGrid.ChunkSize;

    protected void Start()
    {
        OnStart();
    }

    protected virtual void OnStart()
    {
        // Create the ChunkGrid
        ChunkGridFactory<Data> factory = new(gridSize, chunkSize);
        this.chunkGrid = factory.Create();

        // Create Chunk objects with chunk behaviour for each chunk
        IChunk<Data>[,,] chunks = this.chunkGrid.Chunks;
        chunkObjects = new GameObject[chunkSize.Item1, chunkSize.Item2, chunkSize.Item3];
        for (int x = 0; x < chunks.GetLength(0); x++)
            for (int y = 0; y < chunks.GetLength(0); y++)
                for (int z = 0; z < chunks.GetLength(0); z++)
                {
                    GameObject chunkObject = Instantiate(this.chunkObject);
                    Transform chunkTransform = chunkObject.transform;
                    chunkTransform.parent = this.transform;
                    chunkTransform.SetLocalPositionAndRotation(new Vector3(x, y, z), Quaternion.identity);
                    chunkTransform.localScale = Vector3.one;
                    ChunkBehaviour<Data> chunkBehaviour = chunkObject.GetComponent(typeof(ChunkBehaviour<Data>)) as ChunkBehaviour<Data>;
                    chunkBehaviour.chunk = chunks[x,y,z];
                    chunkObjects[x, y, z] = chunkObject;
                }
    }

    protected void Update() { OnUpdate(); }

    protected virtual void OnUpdate() { }

}
