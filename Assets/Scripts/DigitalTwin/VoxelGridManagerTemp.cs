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
using UnityEngine.InputSystem.Utilities;
using Unity.XR.CoreUtils;

namespace AwARe.DigitalTwin.VoxelMap.MonoBehaviours
{
    public class VoxelGridManagerTemp : MonoBehaviour
    {
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;

        public GameObject boundingBox;
        public GameObject childObject;

        [SerializeField] private GameObject loadMapButton;
        [SerializeField] private GameObject loadRayButton;
        [SerializeField] private GameObject addRayButton;

        [SerializeField] private Vector3 gridSize;
        [SerializeField] private Vector3 chunkSize;
        [SerializeField] private Vector3 start;
        [SerializeField] private Vector3 end;

        private Data.ChunkGrid<VoxelInfo> chunkGrid;
        private GameObject voxelMap;
        private DiscreteRay ray;

        private void Awake()
        {
            var loadVB = loadMapButton.GetComponent<Button>();
            loadVB.onClick.AddListener(OnLoadVoxelMapClick);
            var loadRB = loadRayButton.GetComponent<Button>();
            loadRB.onClick.AddListener(OnLoadLocalDiscreteRayClick);
            var addRB = addRayButton.GetComponent<Button>();
            addRB.onClick.AddListener(OnAddRayToGridClick);
        }

        private void OnEnable()
        {
            loadMapButton.SetActive(true);
            loadRayButton.SetActive(true);
            addRayButton.SetActive(true);
        }

        private void OnDisable()
        {
            loadMapButton.SetActive(false);
            loadRayButton.SetActive(false);
            addRayButton.SetActive(false);
        }

        //private void Update() { }

        //? Dit was eerst een override van een obsolete method die interacteerde 
        //? met planes en er iets creeerde, idk of dit nog nodig is maar kijk maar :)
        private void OnLoadVoxelMapClick()
        {
            if (voxelMap == null)
                SpawnBoxedObject(location, Quaternion.Euler(rotation), scale);
            else
                DestroyBoxedObject();
        }
        private void OnLoadLocalDiscreteRayClick()
        {
            if (voxelMap == null)
                return;

            Transform container, grid, chunk;
            container = voxelMap.transform;
            try
            {
                var gridObject = voxelMap.GetComponent<Container>().childObject;
                grid = gridObject.transform;
                var chunkObject = gridObject.GetComponent<VoxelChunkGrid>().chunkObjects[0,0,0];
                chunk = chunkObject.transform;
            }
            catch
            { Debug.Log($"Getting ChunkGridObject failed"); return; }

            Vector3 ToLocal(Vector3 v) => chunk.InverseTransformPoint(v);

            var start_local = ToLocal(start);
            var end_local = ToLocal(end);

            ray = new DiscreteRay(start_local, end_local, (Point3)gridSize);
            Debug.Log($"Ray Constructed");

            Debug.Log($"WorldPos ChunkGrid: {chunk.GetWorldPose().position}");
        }

        private void OnAddRayToGridClick()
        {
            if (voxelMap == null)
                return;

            foreach (var part in ray)
            {
                chunkGrid[part.Idx] = new VoxelInfo(VoxelValue.Solid);
                Debug.Log($"DiscreteRayTrace cell: {part.Idx}");
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(start, 0.01f);
            Gizmos.DrawSphere(end, 0.01f);
        }

        public void SpawnBoxedObject(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            // Start values
            Point3 gridSize = (Point3)this.gridSize, chunkSize = (Point3)this.chunkSize;

            // Create the ChunkGrid
            VoxelMapFactory factory = new();
            chunkGrid = factory.Create(gridSize, chunkSize);

            // Get spawners and factories
            VoxelMapSpawner voxelMapSpawner = new(chunkGrid, childObject, boundingBox);

            // Set sceneObject to manage
            this.voxelMap = voxelMapSpawner.Instantiate(location, rotation);
            this.voxelMap.transform.localScale = scale;
        }

        public void DestroyBoxedObject()
        {
            Destroy(voxelMap);
            voxelMap = null;
        }
    }
}