using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;

public interface IChunkGridSize : IChunkSize
{
    public (int, int, int) GridSize { get; }
    public (int, int, int) NroChunks { get; }
}

public interface IChunkGrid<Data> : IChunkGridSize
{
    public Data this[int x, int y, int z] {  get; set; }
    public int GetLength(int dim);

    IChunk<Data>[,,] Chunks { get; }
}

public class ChunkGrid<Data> : IChunkGrid<Data>
{
    protected const int dim = VoxelData.dim;
    protected readonly int[] gridSize, chunkSize;
    protected readonly IChunk<Data>[,,] chunks;

    public ChunkGrid(int[] gridSize, int[] chunkSize, IChunk<Data>[,,] chunks)
    {
        this.gridSize = gridSize;
        this.chunkSize = chunkSize;
        this.chunks = chunks;
    }

    public (int, int, int) GridSize { get => (gridSize[0], gridSize[1], gridSize[2]); }
    public (int, int, int) NroChunks { get => (chunks.GetLength(0), chunks.GetLength(1), chunks.GetLength(2)); }
    public (int, int, int) ChunkSize { get => (chunkSize[0], chunkSize[1], chunkSize[2]); }

    public Data this[int x, int y, int z] {
        get
        {
            int cx, cy, cz, lx, ly, lz;
            ((cx, cy, cz), (lx, ly, lz)) = GetChunkAndLocalIdx(( x, y, z ));
            return Chunks[cx, cy, cz][lx, ly, lz];
        }
        set
        {
            int cx, cy, cz, lx, ly, lz;
            ((cx, cy, cz), (lx, ly, lz)) = GetChunkAndLocalIdx((x, y, z));
            Chunks[cx, cy, cz][lx, ly, lz] = value;
        }
    }
    public int GetLength(int dim)
    {
        return gridSize[dim];
    }

    protected ((int, int, int), (int, int, int)) GetChunkAndLocalIdx((int, int, int) idx)
    {
        int x, y, z;
        (x, y, z) = idx;
        (int[] chunkIdx, int[] localIdx) = GetChunkAndLocalIdxArray(new int[] { x, y, z });
        int cx = chunkIdx[0], cy = chunkIdx[1], cz = chunkIdx[2];
        int lx = localIdx[0], ly = localIdx[1], lz = localIdx[2];
        return ((cx, cy, cz), (lx, ly, lz));
    }

    protected (int[], int[]) GetChunkAndLocalIdxArray(int[] idx)
    {
        int dim = idx.Length;
        int[] chunkIdx = new int[dim], localIdx = new int[dim];
        for (int i = 0; i < dim; i++)
            chunkIdx[i] = Math.DivRem(idx[i], chunkSize[i], out localIdx[i]);
        return (chunkIdx, localIdx);
    }

    public IChunk<Data>[,,] Chunks { get => chunks; }
}




