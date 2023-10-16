using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SubsystemsImplementation;

public class ARPointCloudVisualizer : IVisualizer<ARPointCloud>
{
    IVisualizer<NativeSlice<Vector3>?> visualizer;

    public ARPointCloudVisualizer(IVisualizer<NativeSlice<Vector3>?> visualizer)
    {
        this.visualizer = visualizer;
    }

    public void Visualize(ARPointCloud toShow)
    {
        Debug.Log("ARPointCloudV");
        NativeSlice<Vector3>? positions = toShow.positions;
        visualizer.Visualize(positions);
    }

    public void Visualize() => visualizer.Visualize();
}