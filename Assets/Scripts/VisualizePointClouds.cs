using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VisualizePointClouds : MonoBehaviour
{
    [SerializeField] private GameObject ARorigin;
    [SerializeField] private GameObject particlePrefab;

    [SerializeField] private int nroParticalSystems;
    [SerializeField] private Color color;
    [SerializeField] private float scale;

    private ARPointCloudManager aRPointCloudManager;

    private Dictionary<TrackableId, IVisualizer<ARPointCloud>> aRPointCloudVisualizers;
    private Dictionary<TrackableId, GameObject> childParticleObjects;

    private IPointCloudVisualizer[] pointCloudVisualizersA;
    private IVisualizer<NativeSlice<Vector3>?>[] maybeCloudVisualizersA;
    private IVisualizer<ARPointCloud>[] aRPointCloudVisualizersA;
    private IVisualizer<ARPointCloudManager> aRPointCloudManagerVisualizer;

    private void Start()
    {
        ARorigin = this.gameObject;
        aRPointCloudManager = ARorigin.GetComponent<ARPointCloudManager>();

        pointCloudVisualizersA = new PointCloudVisualizer_Particles_Basic[nroParticalSystems];
        maybeCloudVisualizersA = new HideOnNullVisualizer<NativeSlice<Vector3>>[nroParticalSystems];
        aRPointCloudVisualizersA = new ARPointCloudVisualizer[nroParticalSystems];

        for (int i = 0; i < nroParticalSystems; i++)
        {
            CreatePointCloudVisualizer(out IPointCloudVisualizer pcV, out IVisualizer<NativeSlice<Vector3>?> mcV, out IVisualizer<ARPointCloud> arpcV);
            pointCloudVisualizersA[i] = pcV; maybeCloudVisualizersA[i] = mcV; aRPointCloudVisualizersA[i] = arpcV;
        }

        var trackableCollectionVisualizer = new TrackableCollectionVisualizer<ARPointCloud>(aRPointCloudVisualizersA);

        aRPointCloudManagerVisualizer = new ARPointCloudManagerVisualizer(trackableCollectionVisualizer);

        InvokeRepeating("Visualize", 5.0f, 2.0f);

        /*
        aRPointCloudManager.pointCloudsChanged += (args) =>
        {
            List<ARPointCloud> added = args.added;
            List<ARPointCloud> updated = args.updated;
            List<ARPointCloud> removed = args.removed;

            foreach (var cloud in added)
                AddPointCloud(cloud);
            foreach (var cloud in added)
                UpdatePointCloud(cloud);
            foreach (var cloud in added)
                RemovePointCloud(cloud);
        };
        */
    }

    protected void AddPointCloud(ARPointCloud cloud)
    {
        CreatePointCloudVisualizer(out GameObject childParticleObject, out IVisualizer<ARPointCloud> aRPointCloudVisualizer);

        var id = cloud.trackableId;
        childParticleObjects.Add(id, childParticleObject);
        aRPointCloudVisualizers.Add(id, aRPointCloudVisualizer);

        aRPointCloudVisualizer.Visualize(cloud);
    }

    protected void UpdatePointCloud(ARPointCloud cloud)
    {
        var id = cloud.trackableId;
        IVisualizer<ARPointCloud> aRPointCloudVisualizer;
        try { aRPointCloudVisualizer = aRPointCloudVisualizers[id]; }
        catch { AddPointCloud(cloud); return; }
        aRPointCloudVisualizer.Visualize(cloud);
    }

    protected void RemovePointCloud(ARPointCloud cloud)
    {
        var id = cloud.trackableId;
        childParticleObjects.Remove(id);
        aRPointCloudVisualizers.Remove(id);
    }

    public void CreatePointCloudVisualizer(out GameObject childParticleObject, out IVisualizer<ARPointCloud> aRPointCloudVisualizer)
    {
        CreatePointCloudVisualizer(out childParticleObject, out IPointCloudVisualizer pointCloudVisualizer, out IVisualizer<NativeSlice<Vector3>?> maybeCloudVisualizer, out aRPointCloudVisualizer);
    }

    public void CreatePointCloudVisualizer(out IPointCloudVisualizer pointCloudVisualizer, out IVisualizer<NativeSlice<Vector3>?> maybeCloudVisualizer, out IVisualizer<ARPointCloud> aRPointCloudVisualizer)
    {
        CreatePointCloudVisualizer(out GameObject childParticleObject, out pointCloudVisualizer, out maybeCloudVisualizer, out aRPointCloudVisualizer);
    }

    public void CreatePointCloudVisualizer(out GameObject childParticleObject, out IPointCloudVisualizer pointCloudVisualizer, out IVisualizer<NativeSlice<Vector3>?> maybeCloudVisualizer, out IVisualizer<ARPointCloud> aRPointCloudVisualizer)
    {
        //childParticleObject = null;
        childParticleObject = Instantiate(particlePrefab, this.transform);
        var particleSystem = childParticleObject.GetComponent<ParticleSystem>();
        //pointCloudVisualizer = new PointCloudVisualizer_Gizmo_Basic(color, scale);
        pointCloudVisualizer = new PointCloudVisualizer_Particles_Basic(particleSystem, color, scale);
        maybeCloudVisualizer = new HideOnNullVisualizer<NativeSlice<Vector3>>(pointCloudVisualizer);
        aRPointCloudVisualizer = new ARPointCloudVisualizer(maybeCloudVisualizer);
    }

    private void Update()
    {
    }

    /*
    private void OnDrawGizmos()
    {
        Debug.Log("OnDrawGizmos");
        Gizmos.color = Color.red;
        var size = new Vector3(this.scale, this.scale, this.scale);
        for (int i = 0; i < nroParticalSystems; i++)
        {
            var gizmoVisualizer = (PointCloudVisualizer_Gizmo_Basic)pointCloudVisualizersA[i];
            var positions = gizmoVisualizer.Positions;
            if (positions == null) continue;
            Debug.Log("OnDrawGizmos: positions.Length = " + positions.Length);
            for (int j = 0; j < positions.Length; j++)
            {
                Debug.Log("OnDrawGizmos: position = " + positions[j]);
                Gizmos.DrawSphere(positions[j], 0.05f);
            }
        }
    }
    */

    protected void Visualize()
    {
        aRPointCloudManagerVisualizer.Visualize(aRPointCloudManager);
        //aRPointCloudManager.pointCloudsChanged

        /*
        int i = -1;
        foreach (var trackable in aRPointCloudManager.trackables)
        {
            if (++i >= nroParticalSystems)
                break;

            UpdateTrackable(trackable, i);
        }
        */
    }
    /*
    public void UpdateTrackable(ARPointCloud trackable, int idx)
    {
        NativeSlice<Vector3>? positions = trackable.positions;
        maybeCloudVisualizersA[idx].Visualize(positions);
    }
    */
}
