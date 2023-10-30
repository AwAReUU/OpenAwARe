using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AwARe.DataTypes;
using Data = AwARe.DataStructures;

namespace AwARe.MonoBehaviours
{
    public class ChunkGrid<T> : MonoBehaviour, Data.IChunkGridSize
    {
        public GameObject chunkObject;

        public Data.IChunkGrid<T> chunkGrid;
        public GameObject[,,] chunkObjects;

        public Point3 GridSize => chunkGrid.GridSize;

        public Point3 NroChunks => chunkGrid.NroChunks;

        public Point3 ChunkSize => chunkGrid.ChunkSize;

        protected void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            // Create Chunk objects with chunk behaviour for each chunk
            Data.IChunk<T>[,,] data = this.chunkGrid.Chunks;
            chunkObjects = new GameObject[data.GetLength(0), data.GetLength(1), data.GetLength(2)];
            for (int x = 0; x < data.GetLength(0); x++)
                for (int y = 0; y < data.GetLength(1); y++)
                    for (int z = 0; z < data.GetLength(2); z++)
                    {
                        GameObject chunkObject = Instantiate(this.chunkObject, this.transform);
                        chunkObject.transform.SetLocalPositionAndRotation(ChunkLocation(new Point3(x, y, z)), Quaternion.identity);
                        chunkObject.transform.localScale = Vector3.one;
                        Chunk<T> chunk = chunkObject.GetComponent<Chunk<T>>();
                        chunk.chunk = data[x, y, z];
                        chunkObjects[x, y, z] = chunkObject;
                    }
        }

        protected Vector3 ChunkLocation(Point3 idx)
        {
            return (this.ChunkSize * idx).ToVector3();
        }
    }
}