using UnityEngine;

namespace AwARe.DigitalTwin.VoxelMap
{
    public class ARInfo
    {
        public Vector3 cameraPosition;
        public Vector3[] pointCloudPositions;
        public float[] pointCloudConfidenceValues;

        public ARInfo(Vector3 cameraPosition, Vector3[] pointCloudPositions, float[] pointCloudConfidenceValues)
        {
            this.cameraPosition = cameraPosition;
            this.pointCloudPositions = pointCloudPositions;
            this.pointCloudConfidenceValues = pointCloudConfidenceValues;
        }
    }

    public class ScanInfo
    {
        public float pointHits;
        public float rayHits;

        public ScanInfo(float pointHits, float rayHits)
        {
            this.pointHits = pointHits;
            this.rayHits = rayHits;
        }
    }

    public class VoxelInfo
    {
        public VoxelValue value;
    }

    public enum VoxelValue { Air, Solid, Unseen, Unknown }
}