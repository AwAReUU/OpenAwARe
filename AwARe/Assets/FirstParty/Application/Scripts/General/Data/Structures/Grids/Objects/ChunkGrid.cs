using AwARe.Data.Logic;

using UnityEngine;

namespace AwARe.Data.Objects
{
    public class ChunkGrid<T> : MonoBehaviour, IChunkGridSize
    {
        public GameObject chunkObject;

        public IChunkGrid<T> chunkGrid;
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
            IChunk<T>[,,] chunks = this.chunkGrid.Chunks;
            chunkObjects = new GameObject[chunks.GetLength(0), chunks.GetLength(1), chunks.GetLength(2)];
            for (int x = 0; x < chunks.GetLength(0); x++)
                for (int y = 0; y < chunks.GetLength(1); y++)
                    for (int z = 0; z < chunks.GetLength(2); z++)
                    {
                        GameObject chunkObject = Instantiate(this.chunkObject, this.transform);
                        chunkObject.transform.SetLocalPositionAndRotation(ChunkLocation(new Point3(x, y, z)), Quaternion.identity);
                        chunkObject.transform.localScale = Vector3.one;
                        AwARe.Data.Objects.Chunk<T> chunkBehaviour = chunkObject.GetComponent<AwARe.Data.Objects.Chunk<T>>();
                        chunkBehaviour.chunk = chunks[x, y, z];
                        chunkObjects[x, y, z] = chunkObject;
                    }
        }

        protected Vector3 ChunkLocation(Point3 idx)
        {
            return (this.ChunkSize * idx).ToVector3();
        }
    }
}