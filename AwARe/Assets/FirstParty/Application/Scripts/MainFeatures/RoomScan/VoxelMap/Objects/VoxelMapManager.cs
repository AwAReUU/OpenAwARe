using AwARe.RoomScan.VoxelMap.Logic;

using AwARe.Data;
using AwARe.Data.Objects;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.RoomScan.VoxelMap.Objects
{
    public class VoxelMapManager : MonoBehaviour
    {
        // Data
        ARPointCloudManager arPointCloudManager;
        Transform cameraTransform;
        IChunkGrid<ScanInfo> scanInfo;
        IChunkGrid<VoxelInfo> voxelInfo;

        // Calculators
        IPointCalculator pointCalculator;
        IScanCalculator scanCalculator;
        IVoxelCalculator voxelCalculator;

        // Unity environment
        VoxelMapSpawner spawner;
    }
}

