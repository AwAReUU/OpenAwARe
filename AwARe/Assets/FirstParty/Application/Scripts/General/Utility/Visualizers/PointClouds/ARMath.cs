using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.Visualizers
{
    public class ARPointCloudInfo<Pos, Id, Conf>
    {
        public Pos positions;
        public Id identifiers;
        public Conf confidenceValues;

        public ARPointCloudInfo(Pos positions, Id identifiers, Conf confidenceValues)
        {
            this.positions = positions;
            this.identifiers = identifiers;
            this.confidenceValues = confidenceValues;
        }
    }

    public static class ARMath
    {
        public static ARPointCloudInfo<Vector3[], ulong[], float[]> GetAllPoints(ARPointCloudManager manager)
        {
            var clouds = new List<ARPointCloud>();
            foreach (var cloud in manager.trackables)
                clouds.Add(cloud);

            var cloudInfos = clouds.Select(x => GetAllPoints(x));
            var positions = cloudInfos.Select(x => x.positions).SelectMany(x => x).ToArray();
            var identifiers = cloudInfos.Select(x => x.identifiers).SelectMany(x => x).ToArray();
            var confidenceValues = cloudInfos.Select(x => x.confidenceValues).SelectMany(x => x).ToArray();
            return new(positions, identifiers, confidenceValues);
        }

        public static int CountPoints(ARPointCloud pointcloud) =>
            pointcloud.positions?.Length ?? 0;

        public static int CountPoints(ARPointCloudInfo<Vector3[], ulong[], float[]> pointcloud) =>
            pointcloud.positions.Length;

        public static ARPointCloudInfo<Vector3[], ulong[], float[]> GetAllPoints(ARPointCloud pointcloud) =>
            new(
                pointcloud.positions?.ToArray() ?? new Vector3[0],
                pointcloud.identifiers?.ToArray() ?? new ulong[0],
                pointcloud.confidenceValues?.ToArray() ?? new float[0]
            );


    }
}
