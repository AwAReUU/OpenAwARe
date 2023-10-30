using UnityEngine;

using AwARe.DataTypes;
using AwARe.DataStructures;

namespace AwARe.DigitalTwin.VoxelMap
{
    public class VoxelMapFactory : ChunkGridFactory<VoxelInfo>
    {
        readonly ChunkGridFactory<VoxelInfo> factory;

        public VoxelMapFactory()
        {
            var chunkFactory = new VoxelChunkFactory();
            this.factory = new ChunkGridFactory<VoxelInfo>(chunkFactory);
        }

        public override ChunkGrid<VoxelInfo> Create(Point3 gridSize, Point3 chunkSize)
        {
            return factory.Create(gridSize, chunkSize);
        }
    }

    public class VoxelChunkFactory : ChunkFactory<VoxelInfo>
    {
        public VoxelChunkFactory() { }

        public override Chunk<VoxelInfo> Create(Point3 chunkSize)
        {
            var data = new VoxelInfo[chunkSize.x, chunkSize.y, chunkSize.z];
            for (int x = 0; x < chunkSize.x; x++)
                for (int y = 0; y < chunkSize.y; y++)
                    for (int z = 0; z < chunkSize.z; z++)
                        data[x, y, z] = new VoxelInfo();
            return new Chunk<VoxelInfo>(data);
        }
    }
}