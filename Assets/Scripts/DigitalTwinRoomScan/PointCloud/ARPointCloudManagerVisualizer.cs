using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

