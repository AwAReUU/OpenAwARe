using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARCore;

public interface IChunkGrid<Data>
{
    public Data this[int x, int y, int z] {  get; set; }

    IChunk<Data>[,,] Chunks { get; }
}

public class ChunkGrid<Data> : IChunkGrid<Data>
{
    const int dim = VoxelData.dim;
    int[] gridSize, chunkSize;
    IChunk<Data>[,,] chunks;


    public ChunkGrid(int[] gridSize, int[] chunkSize, IChunk<Data>[,,] chunks)
    {
        this.gridSize = gridSize;
        this.chunkSize = chunkSize;
        this.chunks = chunks;
    }

    public Data this[int x, int y, int z] {
        get
        {
            int cx, cy, cz, lx, ly, lz;
            ((cx, cy, cz), (lx, ly, lz)) = getChunkAndLocalIdx(( x, y, z ));
            return Chunks[cx, cy, cz][lx, ly, lz];
        }
        set
        {
            int cx, cy, cz, lx, ly, lz;
            ((cx, cy, cz), (lx, ly, lz)) = getChunkAndLocalIdx((x, y, z));
            Chunks[cx, cy, cz][lx, ly, lz] = value;
        }
    }

    protected ((int, int, int), (int, int, int)) getChunkAndLocalIdx((int, int, int) idx)
    {
        int x, y, z;
        (x, y, z) = idx;
        (int[] chunkIdx, int[] localIdx) = getChunkAndLocalIdxArray(new int[] { x, y, z });
        int cx = chunkIdx[0], cy = chunkIdx[1], cz = chunkIdx[2];
        int lx = localIdx[0], ly = localIdx[1], lz = localIdx[2];
        return ((cx, cy, cz), (lx, ly, lz));
    }

    protected (int[], int[]) getChunkAndLocalIdxArray(int[] idx)
    {
        int dim = idx.Length;
        int[] chunkIdx = new int[dim], localIdx = new int[dim];
        for (int i = 0; i < dim; i++)
            chunkIdx[i] = Math.DivRem(idx[i], chunkSize[i], out localIdx[i]);
        return (chunkIdx, localIdx);
    }

    public IChunk<Data>[,,] Chunks { get => chunks; }
}




