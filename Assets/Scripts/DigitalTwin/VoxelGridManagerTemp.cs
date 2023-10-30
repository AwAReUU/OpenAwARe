using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

using AwARe.DataTypes;
using AwARe.MonoBehaviours;
using Data = AwARe.DataStructures;
using System;
using AwARe.DataStructures;
using UnityEditor.Experimental.GraphView;

namespace AwARe.DigitalTwin.VoxelMap.MonoBehaviours
{
    public class VoxelGridManagerTemp : MonoBehaviour
    {
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;

        public GameObject boundingBox;
        public GameObject childObject;

        [SerializeField] private GameObject loadButton;
        public GameObject sceneBox;

        IEnumerator<float> parameterEnumerator;
        IEnumerator<CellRay> segmentEnumerator;
        int i = 0, imax = 100;

        private void Awake()
        {
            var loadB = loadButton.GetComponent<Button>();
            loadB.onClick.AddListener(OnLoadVoxelMapClick);
        }

        private void OnEnable()
        {
            loadButton.SetActive(true);

            Debug.Log($"Setup Discrete Ray");
            Point3 gridSize = new Point3(16, 16, 16);
            var end = new Vector3(0, -2, 0);
            var start = new Vector3(18, 5, 9);
            var ray = new DiscreteRay(start, end, gridSize);

            parameterEnumerator = new DiscreteRayParameterEnumerator(ray);
            parameterEnumerator.Reset();

            segmentEnumerator = new DiscreteRayEnumerator(ray);
            segmentEnumerator.Reset();

            Debug.Log($"start: {ray.start}");
            Debug.Log($"end: {ray.end}");
            Debug.Log($"l_start: {ray.l_start}");
            Debug.Log($"l_end: {ray.l_end}");
        }

        private void OnDisable()
        {
            loadButton.SetActive(false);
        }

        private void Update()
        {
            /*
            Debug.Log($"...Temp: Update");
            if (parameterEnumerator.MoveNext() && ++i < imax)
            {
                var l = parameterEnumerator.Current;
                Debug.Log($"DiscreteRayTrace l: {l}");
            }
            */

            Debug.Log($"...Temp: Update");
            if (segmentEnumerator.MoveNext())
            {
                var part = segmentEnumerator.Current;
                Debug.Log($"DiscreteRayTrace cell: {part.Idx}");
            }
        }

        //? Dit was eerst een override van een obsolete method die interacteerde 
        //? met planes en er iets creeerde, idk of dit nog nodig is maar kijk maar :)
        private void OnLoadVoxelMapClick()
        {
            if (sceneBox == null)
                SpawnBoxedObject(location, Quaternion.Euler(rotation), scale);
            else
                DestroyBoxedObject();
        }

        public void SpawnBoxedObject(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            // Start values
            Point3 gridSize = new Point3(16, 16, 16), chunkSize = new Point3(4, 4, 4);

            // Create the ChunkGrid
            VoxelMapFactory factory = new();
            Data.ChunkGrid<VoxelInfo> chunkGrid = factory.Create(gridSize, chunkSize);

            // Get spawners and factories
            VoxelMapSpawner voxelMapSpawner = new(chunkGrid, childObject, boundingBox);

            // Set sceneObject to manage
            //this.sceneBox = voxelMapSpawner.Instantiate(location, rotation);
            //this.sceneBox.transform.localScale = scale;

            var start = new Vector3(0, 1, 0);
            var end = new Vector3(16, 15, 16);
            var ray = new DiscreteRay(start, end, gridSize);

            int i = 0, imax = 20;
            foreach(var part in ray)
            {
                if(++i >= imax) break;
                //chunkGrid[part.Idx] = new VoxelInfo(VoxelValue.Solid);
                Debug.Log($"DiscreteRayTrace cell: {part.Idx}");
            }
        }

        public void DestroyBoxedObject()
        {
            Destroy(sceneBox);
            sceneBox = null;
        }
    }
}