using System.Collections;

using AwARe.Visualizers;

using Unity.Collections;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AwARe.RoomScan.VoxelMap.Objects
{
    public class RoomScanVizualization : MonoBehaviour
    {
        [SerializeField] private GameObject arOrigin;

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

        private void Start()
        {
            // Get PointCloudManager stored in AR Origin
            arOrigin = GameObject.Find("AR Session Origin"); if (arOrigin == null) return;
            arPointCloudManager = arOrigin.GetComponent<ARPointCloudManager>(); if (arPointCloudManager == null) return;

            // Initialize Vizualizers
            InitializeVisualizers();

            // Start Visualization
            StartCoroutine(VisualizeRoutine());
            visualsActive = true;
        }

        protected void InitializeVisualizers()
        {
            particleObjects = new GameObject[nroParticalSystems];
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
    }
}