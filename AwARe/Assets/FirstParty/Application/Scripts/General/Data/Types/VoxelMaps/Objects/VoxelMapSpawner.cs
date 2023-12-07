using AwARe.Objects;
using AwARe.RoomScan.VoxelMap.Logic;

using UnityEngine;

namespace AwARe.Data.Objects
{
    public class VoxelMapSpawner : CompositeSpawner
    {
        public VoxelMapSpawner(Logic.ChunkGrid<VoxelInfo> chunkGrid, GameObject chunkGridPrefab, GameObject boundingBoxPrefab)
        {
            // Get spawners and factories
            var chunkGridSpawner = new ChunkGridSpawner<VoxelInfo>(chunkGridPrefab, chunkGrid);
            var boundingBoxSpawner = new ContainerSpawner(boundingBoxPrefab, chunkGridSpawner);

            // Set base spawner
            spawner = boundingBoxSpawner;
        }
    }
}