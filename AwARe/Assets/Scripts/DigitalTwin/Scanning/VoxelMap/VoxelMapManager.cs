using UnityEngine;
using UnityEngine.XR.ARFoundation;

using Data = AwARe.DataStructures;
using AwARe.MonoBehaviours;

namespace AwARe.DigitalTwin.VoxelMap.MonoBehaviours
{
    public class VoxelMapManager : MonoBehaviour
    {
        // Data
        ARPointCloudManager arPointCloudManager;
        Transform cameraTransform;
        Data.IChunkGrid<ScanInfo> scanInfo;
        Data.IChunkGrid<VoxelInfo> voxelInfo;

        // Calculators
        IPointCalculator pointCalculator;
        IScanCalculator scanCalculator;
        IVoxelCalculator voxelCalculator;

        // Unity environment
        VoxelMapSpawner spawner;

        void Start()
        {

        }

        void Update()
        {

        }
    }
}

