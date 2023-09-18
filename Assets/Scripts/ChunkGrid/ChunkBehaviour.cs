using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkBehaviour<Data> : MonoBehaviour
{
    public IChunk<Data> chunk;

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
