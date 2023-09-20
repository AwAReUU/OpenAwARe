using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGridBehavior<Data> : MonoBehaviour, IChunkGridSize
{
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
        // Create Chunk objects with chunk behaviour for each chunk
        IChunk<Data>[,,] chunks = this.chunkGrid.Chunks;
        chunkObjects = new GameObject[chunks.GetLength(0), chunks.GetLength(1), chunks.GetLength(2)];
        for (int x = 0; x < chunks.GetLength(0); x++)
            for (int y = 0; y < chunks.GetLength(1); y++)
                for (int z = 0; z < chunks.GetLength(2); z++)
                {
                    GameObject chunkObject = Instantiate(this.chunkObject, this.transform);
                    chunkObject.transform.SetLocalPositionAndRotation(ChunkLocation((x, y, z)), Quaternion.identity);
                    chunkObject.transform.localScale = Vector3.one;
                    ChunkBehaviour<Data> chunkBehaviour = chunkObject.GetComponent(typeof(ChunkBehaviour<Data>)) as ChunkBehaviour<Data>;
                    chunkBehaviour.chunk = chunks[x,y,z];
                    chunkObjects[x, y, z] = chunkObject;
                }
    }

    protected void Update() { OnUpdate(); }

    protected virtual void OnUpdate() { }

    protected Vector3 ChunkLocation((int,int,int) idx)
    {
        (int, int, int) chunkSize = this.ChunkSize;
        return new Vector3(chunkSize.Item1 * idx.Item1, chunkSize.Item1 * idx.Item2, chunkSize.Item1 * idx.Item3);
    }
}
