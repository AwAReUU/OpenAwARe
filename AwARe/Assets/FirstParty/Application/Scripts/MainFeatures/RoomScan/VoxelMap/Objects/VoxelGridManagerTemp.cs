using AwARe.Data.Logic;
using L = AwARe.Data.Logic;
using AwARe.Data.Objects;
using AwARe.RoomScan.VoxelMap.Logic;

using UnityEngine;

namespace AwARe.RoomScan.VoxelMap.Objects
{
    public class VoxelGridManagerTemp : MonoBehaviour
    {
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;

        public GameObject boundingBox;
        public GameObject childObject;

        public GameObject sceneBox;

        //? Dit was eerst een override van een obsolete method die interacteerde 
        //? met planes en er iets creeerde, idk of dit nog nodig is maar kijk maar :)
        private void Interact()
        {
            if (sceneBox == null)
                SpawnBoxedObject(location, Quaternion.Euler(rotation), scale);
            else
                DestroyBoxedObject();
        }

        public void SpawnBoxedObject(Vector3 location, Quaternion rotation, Vector3 scale)
        {
            // Start values
            Point3 gridSize = new Point3(64, 64, 64), chunkSize = new Point3(16, 16, 16);

            // Create the ChunkGrid
            ChunkGridFactory<VoxelInfo> factory = new();
            L.ChunkGrid<VoxelInfo> chunkGrid = factory.Create(gridSize, chunkSize);

            // Get spawners and factories
            VoxelMapSpawner voxelMapSpawner = new(chunkGrid, childObject, boundingBox);

            // Set sceneObject to manage
            this.sceneBox = voxelMapSpawner.Instantiate(location, rotation);
            this.sceneBox.transform.localScale = scale;
        }

        public void DestroyBoxedObject()
        {
            Destroy(sceneBox);
            sceneBox = null;
        }
    }
}