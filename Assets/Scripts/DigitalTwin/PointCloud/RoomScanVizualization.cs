
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;

using AwARe.Visualization;

namespace AwARe.DigitalTwin.VoxelMap.MonoBehaviours
{
    public class RoomScanVizualization : MonoBehaviour
    {
        [SerializeField] private GameObject ARorigin;
        [SerializeField] private GameObject particlePrefab;

        [SerializeField] private int nroParticalSystems;
        [SerializeField] private Color color;
        [SerializeField] private float scale;
        [SerializeField] private float visualizeDelay;

        private ARPointCloudManager arPointCloudManager;
        public bool visualsActive = false;

        private GameObject[] particleObjects;
        private IPointCloudVisualizer[] pointCloudVisualizers;
        private IVisualizer<ARPointCloud>[] arPointCloudVisualizers;
        private IVisualizer<ARPointCloudManager> visualizer;

        //private Dictionary<TrackableId, IVisualizer<ARPointCloud>> aRPointCloudVisualizers;
        //private Dictionary<TrackableId, GameObject> childParticleObjects;


        private void Start()
        {
            // Get PointCloudManager stored in AR Origin
            var arSessionOrigin = GameObject.Find("AR Session Origin"); if (arSessionOrigin == null) return;
            arPointCloudManager = arSessionOrigin.GetComponent<ARPointCloudManager>(); if (arPointCloudManager == null) return;

            // TODO Replace with factories
            pointCloudVisualizers = new IPointCloudVisualizer[nroParticalSystems];
            arPointCloudVisualizers = new ARPointCloudVisualizer[nroParticalSystems];
            for (int i = 0; i < nroParticalSystems; i++)
            {
                particleObjects[i] = Instantiate(particlePrefab, this.transform);
                var particleSystem = particleObjects[i].GetComponent<ParticleSystem>();
                pointCloudVisualizers[i] = new PointCloudVisualizer_Particles_Basic(particleSystem, color, scale);
                var nullableVizualizer = new HideOnNullVisualizer<NativeSlice<Vector3>>(pointCloudVisualizers[i]);
                arPointCloudVisualizers[i] = new ARPointCloudVisualizer(nullableVizualizer);
            }
            var trackableVizualizer = new TrackableCollectionVisualizer<ARPointCloud>(arPointCloudVisualizers);
            visualizer = new ARPointCloudManagerVisualizer(trackableVizualizer);

            // Start Visualization
            StartCoroutine(VisualizeRoutine());

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

            visualsActive = true;
        }

        IEnumerator VisualizeRoutine()
        {
            Debug.Log("Routine Started");
            float alpha = 0;
            while (true)
            {
                alpha += Time.deltaTime;
                if (alpha > visualizeDelay)
                {
                    alpha = 0;
                    Visualize();
                    Debug.Log("Visualize Fired");
                }
                yield return null;
            }
        }
        protected void Visualize()
        {
            visualizer.Visualize(arPointCloudManager);
        }





        /*
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
        */

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

        /*
        public void UpdateTrackable(ARPointCloud trackable, int idx)
        {
            NativeSlice<Vector3>? positions = trackable.positions;
            maybeCloudVisualizersA[idx].Visualize(positions);
        }
        */
    }
}