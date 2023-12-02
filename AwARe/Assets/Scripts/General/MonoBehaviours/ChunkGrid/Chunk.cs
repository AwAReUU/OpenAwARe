using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Data = AwARe.DataStructures;
using AwARe.DataTypes;

namespace AwARe.MonoBehaviours
{
    public class Chunk<T> : MonoBehaviour, Data.IChunkSize
    {
        public Data.IChunk<T> chunk;

        public Point3 ChunkSize => chunk.ChunkSize;

        protected void Start()
        {
            OnStart();
        }

        protected virtual void OnStart() { }

        protected void Update()
        {
            OnUpdate();
        }
        protected virtual void OnUpdate()
        {
            if (!chunk.Changed)
                return;

            OnChange();
            chunk.Changed = false;
        }

        protected virtual void OnChange() { }
    }

}