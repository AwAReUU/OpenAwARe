using AwARe.Data;
using AwARe.RoomScan.VoxelMap.Logic;

namespace AwARe.RoomScan.VoxelMap
{
    public interface IPointCalculator
    {
        public ARInfo GetInfo();
    }

    public interface IScanCalculator
    {
        public void UpdateScanInfo(ARInfo arInfo, IChunkGrid<ScanInfo> scanInfo);
    }

    public interface IVoxelCalculator
    {
        public void UpdateVoxelInfo(IChunkGrid<ScanInfo> scanInfo, IChunkGrid<VoxelInfo> voxelInfo);
    }
}

