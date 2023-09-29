using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SubsystemsImplementation;

public interface IVisualizable<In>
{
    public IVisualizer<In> GetVisualizer();
}

public interface IVisualizer<In>
{
    public void Visualize(In toShow);
}

public class PointCloudVisualizer_Particles_Basic : IVisualizer<NativeArray<Vector3>>, IVisualizer<NativeSlice<Vector3>>, IVisualizer<Vector3[]>
{
    private ParticleSystem particleSystem;
    private Color color;
    private float scale;

    public PointCloudVisualizer_Particles_Basic(ParticleSystem particleSystem, Color color, float scale)
    {
        this.particleSystem = particleSystem;
        this.color = color;
        this.scale = scale;
    }

    public void Visualize(NativeArray<Vector3> toShow)
    {
        int l = toShow.Length;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];

        for (int i = 0; i < l; i++)
        {
            particles[i].position = toShow[i];
            particles[i].startColor = color;
            particles[i].startSize = scale;
        }

        particleSystem.SetParticles(particles, l);
    }

    public void Visualize(NativeSlice<Vector3> toShow)
    {
        int l = toShow.Length;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];

        for (int i = 0; i < l; i++)
        {
            particles[i].position = toShow[i];
            particles[i].startColor = color;
            particles[i].startSize = scale;
        }

        particleSystem.SetParticles(particles, l);
    }

    public void Visualize(Vector3[] toShow)
    {
        int l = toShow.Length;
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[l];

        for (int i = 0; i < l; i++)
        {
            particles[i].position = toShow[i];
            particles[i].startColor = color;
            particles[i].startSize = scale;
        }

        particleSystem.SetParticles(particles, l);
    }
}

[RequireComponent(typeof(ParticleSystem))]
public class ARPointCloudVisualizer : IVisualizer<ARPointCloud>
{
    IVisualizer<NativeSlice<Vector3>> visualizer;

    public ARPointCloudVisualizer(IVisualizer<NativeSlice<Vector3>> visualizer)
    {
        this.visualizer = visualizer;
    }

    public void Visualize(ARPointCloud toShow)
    {
        NativeSlice<Vector3>? positions = toShow.positions;

        if (!positions.HasValue)
            return;

        visualizer.Visualize(positions.Value);
    }
}

public class GroupVisualizer<In> : IVisualizer<In[]>
{
    IVisualizer<In>[] visualizers;

    public GroupVisualizer(IVisualizer<In>[] visualizers)
    {
        this.visualizers = visualizers;
    }

    public void Visualize(In[] toShow)
    {
        int l = Math.Max(toShow.Length, visualizers.Length);
        for (int i = 0; i < l; i++)
            visualizers[i].Visualize(toShow[i]);
    }
}

public class TrackableCollectionVisualizer<In> : IVisualizer<TrackableCollection<In>>
{
    IVisualizer<In>[] visualizers;

    public TrackableCollectionVisualizer(IVisualizer<In>[] visualizers)
    {
        this.visualizers = visualizers;
    }

    public void Visualize(TrackableCollection<In> toShow)
    {
        int l = visualizers.Length;
        int i = -1;
        foreach (var showable in toShow)
        {
            if (++i < l) break;
            visualizers[i].Visualize(showable);
        }
    }
}

public class ARPointCloudManagerVisualizer : IVisualizer<ARPointCloudManager>
{
    IVisualizer<TrackableCollection<ARPointCloud>> visualizer;

    public ARPointCloudManagerVisualizer(IVisualizer<TrackableCollection<ARPointCloud>> visualizer)
    {
        this.visualizer = visualizer;
    }

    public void Visualize(ARPointCloudManager toShow)
    {
        TrackableCollection<ARPointCloud> pointClouds = toShow.trackables;
        visualizer.Visualize(pointClouds);
    }
}

public class ARPointCloudVisualizerFactory
{

}

public class ARPointCloudManagerVizualization : MonoBehaviour
{
    [SerializeField] private GameObject ARorigin;
    [SerializeField] private GameObject particlePrefab;

    [SerializeField] private int nroParticalSystems;
    [SerializeField] private Color color;
    [SerializeField] private float scale;

    private ARPointCloudManager aRPointCloudManager;
    private IVisualizer<ARPointCloudManager> aRPointCloudManagerVisualizer;

    private void Start()
    {
        aRPointCloudManager = ARorigin.GetComponent<ARPointCloudManager>();

        var aRPointCloudVisualizers = new ARPointCloudVisualizer[nroParticalSystems];

        for (int i = 0; i < nroParticalSystems; i++)
        {
            Instantiate(particlePrefab, this.transform);
            var particleSystem = particlePrefab.GetComponent<ParticleSystem>();
            var particalVisualizer = new PointCloudVisualizer_Particles_Basic(particleSystem, color, scale);
            aRPointCloudVisualizers[i] = new ARPointCloudVisualizer(particalVisualizer);
        }

        var trackableCollectionVisualizer = new TrackableCollectionVisualizer<ARPointCloud>(aRPointCloudVisualizers);
        
        aRPointCloudManagerVisualizer = new ARPointCloudManagerVisualizer(trackableCollectionVisualizer);
    }

    public void Update()
    {
        aRPointCloudManagerVisualizer.Visualize(aRPointCloudManager);
    }
}

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
