using System;
using System.Linq;
using UnityEngine;

using AwARe.DataStructures;
using UnityEngine.UIElements;

namespace AwARe.DigitalTwin.VoxelMap
{
    public class ScanCalculator : IScanCalculator
    {
        public void UpdateScanInfo(ARInfo arInfo, IChunkGrid<ScanInfo> scanInfo) =>
            UpdateScanInfo(arInfo.cameraPosition, arInfo.pointCloudPositions, arInfo.pointCloudConfidenceValues, scanInfo);

        public void UpdateScanInfo(Vector3 start, Vector3[] ends, float[] confidences, IChunkGrid<ScanInfo> scanInfo)
        {
            int l = Math.Max(ends.Length, confidences.Length);
            for (int i = 0; i < l; i++)
            {
                var end = ends[i];
                var confidence = confidences[i];
                UpdateScanInfo(start, end, confidence, scanInfo);
            }
        }

        public void UpdateScanInfo(Vector3 start, Vector3 end, float confidence, IChunkGrid<ScanInfo> scanInfo)
        {

        }
    }
}

