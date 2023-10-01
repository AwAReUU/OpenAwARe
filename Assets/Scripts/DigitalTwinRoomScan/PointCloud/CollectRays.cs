using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class ARPointCloudVizualization<In> : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private int nroParticalSystems;
    [SerializeField] private Color color;
    [SerializeField] private float scale;

    [SerializeField] private IVisualizer<Vector3[]> pointCloudVisualizer;

    private void Start()
    {
        for (int i = 0; i < nroParticalSystems; i++)
        {
            Instantiate(prefab, this.transform);
            prefab.GetComponent<ParticleSystem>();
        }
    }

}

/*
public class CollectRays : MonoBehaviour
{
    [SerializeField] private GameObject ARSessionOrigin;
    private ARPointCloudManager pointCloudManager;
    [SerializeField] private GameObject ARCamera;



    // Start is called before the first frame update
    void Start()
    {
        pointCloudManager = ARSessionOrigin?.AddComponent(typeof(ARPointCloudManager)) as ARPointCloudManager;
        particleSystem = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        TrackableCollection<ARPointCloud> pointClouds;
        pointClouds = pointCloudManager.trackables;

        foreach (var pointCloud in pointClouds)
        {
            NativeSlice<Vector3>? positions = pointCloud.positions;
            NativeSlice<ulong>? identifiers = pointCloud.identifiers;
            NativeArray<float>? confidenceValues = pointCloud.confidenceValues;

        }
    }




    //XRPointCloudSubsystem

    //SubsystemProvider

*/
