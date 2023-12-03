using UnityEngine;
using AwARe.Data.Logic;

namespace AwARe.Data.Objects
{
    public class Chunk<T> : MonoBehaviour, IChunkSize
    {
        public IChunk<T> chunk;

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