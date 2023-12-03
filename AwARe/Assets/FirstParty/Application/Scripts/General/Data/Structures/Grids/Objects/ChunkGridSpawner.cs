using System;

using AwARe.Objects;

using UnityEngine;

namespace AwARe.Data.Objects
{
    public class ChunkGridSpawner<T> : PrefabSpawner
    {
       Logic.ChunkGrid<T> chunkGrid;

        public ChunkGridSpawner(GameObject prefab, Logic.ChunkGrid<T> chunkGrid) : base(prefab)
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
