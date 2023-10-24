using System;
using UnityEngine;

using Data = AwARe.DataStructures;

namespace AwARe.MonoBehaviours
{
    public class ChunkGridSpawner<T> : PrefabSpawner
    {
        Data.ChunkGrid<T> chunkGrid;

        public ChunkGridSpawner(GameObject prefab, Data.ChunkGrid<T> chunkGrid) : base(prefab)
        {
            this.chunkGrid = chunkGrid;
        }

        protected override GameObject InstantiateHelper(Func<GameObject> instantiate)
        {
            // Instantiate ChunkGridObject and sets Parent
            GameObject chunkGrid = instantiate();

            // Pass ChunkGridObject a ChunkGrid
            var behaviour = chunkGrid.GetComponent<ChunkGrid<T>>();
            behaviour.chunkGrid = this.chunkGrid;

            return chunkGrid;
        }
    }
}
