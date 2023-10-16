
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System;

public class RoomScanVizualization : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    [SerializeField] private int nroParticalSystems;
    [SerializeField] private Color color;
    [SerializeField] private float scale;
    [SerializeField] private float visualizeDelay;

    [SerializeField] private IVisualizer<NativeSlice<Vector3>> pointCloudVisualizer;

    private IVisualizer<ARPointCloudManager> vizualizer;
    private ARPointCloudManager pointCloudManager;

    // Start is called before the first frame update
    void Start()
    {

        pointCloudManager = GameObject.Find("AR Session Origin").GetComponent<ARPointCloudManager>();

        var ARPointCloudVisualizers = new ARPointCloudVisualizer[nroParticalSystems];
        for (int i = 0; i < nroParticalSystems; i++)
        {
            Instantiate(prefab, this.transform);
            var particleSystem = prefab.GetComponent<ParticleSystem>();
            pointCloudVisualizer = new PointCloudVisualizer_Particles_Basic(particleSystem, color, scale);
            var nullableVizualizer = new HideOnNullVisualizer<NativeSlice<Vector3>>(pointCloudVisualizer);
            ARPointCloudVisualizers[i]= new ARPointCloudVisualizer(nullableVizualizer);
        }
        var trackableVizualizer = new TrackableCollectionVisualizer<ARPointCloud>(ARPointCloudVisualizers);
        vizualizer = new ARPointCloudManagerVisualizer(trackableVizualizer);

        VizualizePointCloudManager();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void VizualizePointCloudManager()
    {
        StartCoroutine(VizualizePointCloudManagerRoutine());
    }

    IEnumerator VizualizePointCloudManagerRoutine()
    {
        Debug.Log("Routine Started");
        float alpha = 0;
        while(true)
        {
            alpha += Time.deltaTime;
            if(alpha > visualizeDelay)
            {
                alpha = 0;
                vizualizer.Visualize(pointCloudManager);
                Debug.Log("Visualize Fired");
            }
            yield return null;
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
