using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

using AwARe.DataTypes;

namespace AwARe.DigitalTwin.VoxelMap
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

