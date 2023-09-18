using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARCore;


public interface IChunk<Data>
{
    public Data this[int x, int y, int z] { get; set; }

    public bool Changed { get; set; }

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

    public bool Changed { get => changed; set => changed = value; }

    public int GetLength(int dim) => chunkData.GetLength(dim);

    public Data[,,] ChunkData
    {
        get => chunkData;
    }
}