using UnityEngine;

namespace AwARe.DigitalTwin.VoxelMap
{
    public class ARInfo
    {
        public Vector3 cameraPosition = Vector3.zero;
        public Vector3[] pointCloudPositions = new Vector3[0];
        public float[] pointCloudConfidenceValues = new float[0];

        public ARInfo()
        { }

        // Never alter, if other format/input is desired, implement new constructor or create factory methods.
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

        public VoxelInfo()
        {
            this.value = VoxelValue.Unseen;
        }

        public VoxelInfo(VoxelValue value)
        {
            this.value = value;
        }
    }

    public enum VoxelValue { Unseen, Air, Solid, Unknown, Outside }
}