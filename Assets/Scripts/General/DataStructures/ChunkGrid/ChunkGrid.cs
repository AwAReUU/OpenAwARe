using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

using AwARe.DataTypes;

namespace AwARe.DataStructures
{
    public interface IChunkGridSize : IChunkSize, IGridSize
    {
        public Point3 NroChunks { get; }
    }



    public interface IChunkGrid<T> : IChunkGridSize, IGrid<T>
    {
        IChunk<T>[,,] Chunks { get; }
    }

    public class ChunkGrid<T> : IChunkGrid<T>
    {
        protected const int dim = VoxelData.dim;
        protected readonly Point3 gridSize, chunkSize;
        protected readonly IChunk<T>[,,] chunks;

        public ChunkGrid(Point3 gridSize, Point3 chunkSize, IChunk<T>[,,] chunks)
        {
            this.gridSize = gridSize;
            this.chunkSize = chunkSize;
            this.chunks = chunks;
        }

        public Point3 GridSize { get => gridSize; }
        public Point3 NroChunks { get => new Point3(chunks.GetLength(0), chunks.GetLength(1), chunks.GetLength(2)); }
        public Point3 ChunkSize { get => chunkSize; }

        public T this[int x, int y, int z]
        {
            get
            {
                Point3 c, l;
                (c, l) = GetChunkAndLocalIdx(new Point3(x, y, z));
                return Chunks[c.x, c.y, c.z][l.x, l.y, l.z];
            }
            set
            {
                Point3 c, l;
                (c, l) = GetChunkAndLocalIdx(new Point3(x, y, z));
                Chunks[c.x, c.y, c.z][l.x, l.y, l.z] = value;
            }
        }

        public T this[Point3 idx]
        {
            get
            {
                Point3 c, l;
                (c, l) = GetChunkAndLocalIdx(idx);
                return Chunks[c.x, c.y, c.z][l.x, l.y, l.z];
            }
            set
            {
                Point3 c, l;
                (c, l) = GetChunkAndLocalIdx(idx);
                Chunks[c.x, c.y, c.z][l.x, l.y, l.z] = value;
            }
        }

        public int GetLength(int dim) =>
            gridSize[dim];

        protected (Point3, Point3) GetChunkAndLocalIdx(Point3 idx)
        {
            (int[] ca, int[] la) = GetChunkAndLocalIdxArray(idx.ToArray());
            return (new Point3(ca), new Point3(la));
        }

        protected (int[], int[]) GetChunkAndLocalIdxArray(int[] idx)
        {
            int dim = idx.Length;
            int[] chunkIdx = new int[dim], localIdx = new int[dim];
            for (int i = 0; i < dim; i++)
                chunkIdx[i] = Math.DivRem(idx[i], chunkSize[i], out localIdx[i]);
            return (chunkIdx, localIdx);
        }

        public IChunk<T>[,,] Chunks { get => chunks; }
    }
}




