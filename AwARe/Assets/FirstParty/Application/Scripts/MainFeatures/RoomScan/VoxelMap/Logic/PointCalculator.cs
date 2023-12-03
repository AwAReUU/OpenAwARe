using AwARe.Visualizers;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.RoomScan.VoxelMap.Logic
{
    public class PointCalculator : IPointCalculator
    {
        ARPointCloudManager manager;
        Transform camera;

        public ARInfo GetInfo()
        {
            var arPcInfo = ARMath.GetAllPoints(manager);
            return new ARInfo(camera.position, arPcInfo.positions, arPcInfo.confidenceValues);
        }
    }
}

