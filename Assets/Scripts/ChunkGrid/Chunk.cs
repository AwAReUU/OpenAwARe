using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public interface IChunkSize
{
    public (int, int, int) ChunkSize { get; }
}

public interface IChunkStatus
{
    public bool Changed { get; set; }
}

public interface IChunk<Data> : IChunkStatus, IChunkSize
{
    public Data this[int x, int y, int z] { get; set; }
    public int GetLength(int dim);

    public Data[,,] ChunkData { get; }
}

public class Chunk<Data> : IChunk<Data>
{
    protected const int dim = VoxelData.dim;
    protected Data[,,] chunkData;
    protected bool changed = true;

    public Chunk(Data[,,] chunkData)
    {
        this.chunkData = chunkData;
    }

    public (int, int, int) ChunkSize { get => (chunkData.GetLength(0), chunkData.GetLength(1), chunkData.GetLength(2)); }

    public bool Changed { get => changed; set => changed = value; }

    public Data this[int x, int y, int z]
    {
        get => chunkData[x, y, z];
        set
        {
            if (chunkData[x, y, z].Equals(value))
                return;

            chunkData[x, y, z] = value;
            Changed = true;
        }
    }

    public int GetLength(int dim) => chunkData.GetLength(dim);

    public Data[,,] ChunkData
    {
        get => chunkData;
    }
}