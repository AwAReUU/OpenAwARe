using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.Visualization
{
    public interface IPointCloudVisualizer : IVisualizer<Vector3[]>, IVisualizer<NativeArray<Vector3>>, IVisualizer<NativeSlice<Vector3>>
    { }

    public class ARPointCloudManagerVisualizer : IVisualizer<ARPointCloudManager>
    {
        IVisualizer<TrackableCollection<ARPointCloud>> visualizer;

        public ARPointCloudManagerVisualizer(IVisualizer<TrackableCollection<ARPointCloud>> visualizer)
        {
            this.visualizer = visualizer;
        }

        public void Visualize(ARPointCloudManager toShow)
        {
            Debug.Log("ARPointCloudManagerV");
            TrackableCollection<ARPointCloud> pointClouds = toShow.trackables;
            visualizer.Visualize(pointClouds);
        }

        public void Visualize() => visualizer.Visualize();
    }
}

