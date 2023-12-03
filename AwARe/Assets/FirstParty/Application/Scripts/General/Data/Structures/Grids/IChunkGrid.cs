using System;
using AwARe.Data.Logic;

namespace AwARe.Data
{ 
    public interface IChunkSize
    {
        public Point3 ChunkSize { get; }
    }

    public interface IChangeable
    {
        public bool Changed { get; set; }
    }

    public interface IChunk<T> : IChangeable, IChunkSize
    {
        public T this[int x, int y, int z] { get; set; }
        public int GetLength(int dim);

        public T[,,] Data { get; }
    }

    public interface IChunkGridSize : IChunkSize
    {
        public Point3 GridSize { get; }
        public Point3 NroChunks { get; }
    }

    public interface IChunkGrid<T> : IChunkGridSize
    {
        public T this[int x, int y, int z] { get; set; }
        public int GetLength(int dim);

        IChunk<T>[,,] Chunks { get; }
    }
}