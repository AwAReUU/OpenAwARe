using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

using AwARe.DataTypes;

namespace AwARe.DataStructures
{
    public interface IChunkSize
    {
        public Point3 ChunkSize { get; }
    }

    public interface IChangeable
    {
        public bool Changed { get; set; }
    }

    public interface IChunk<T> : IChangeable, IGrid<T>, IChunkSize
    {
        public T[,,] Data { get; }
    }

    public class Chunk<T> : IChunk<T>
    {
        protected const int dim = VoxelData.dim;
        protected T[,,] data;
        protected bool changed = true;

        public Chunk(T[,,] data)
        {
            this.data = data;
        }

        public Point3 ChunkSize { get => new Point3(data.GetLength(0), data.GetLength(1), data.GetLength(2)); }

        public bool Changed { get => changed; set => changed = value; }

        public T this[int x, int y, int z]
        {
            get => data[x, y, z];
            set
            {
                if (data[x, y, z].Equals(value))
                    return;

                data[x, y, z] = value;
                Changed = true;
            }
        }

        public T this[Point3 idx]
        {
            get => this[idx.x, idx.y, idx.z];
            set => this[idx.x, idx.y, idx.z] = value;
        }

        public int GetLength(int dim) => data.GetLength(dim);

        public T[,,] Data
        {
            get => data;
        }
    }
}