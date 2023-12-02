using UnityEngine;

using AwARe.DataTypes;
using Data = AwARe.DataStructures;
using System;
using AwARe.DigitalTwin.VoxelMap;

namespace AwARe.MonoBehaviours
{
    public class VoxelMapSpawner : CompositeSpawner
    {
        public VoxelMapSpawner(Data.ChunkGrid<VoxelInfo> chunkGrid, GameObject chunkGridPrefab, GameObject boundingBoxPrefab)
        {
            // Get spawners and factories
            var chunkGridSpawner = new ChunkGridSpawner<VoxelInfo>(chunkGridPrefab, chunkGrid);
            var boundingBoxSpawner = new ContainerSpawner(boundingBoxPrefab, chunkGridSpawner);

            // Set base spawner
            spawner = boundingBoxSpawner;
        }
    }
}