using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGridBoundingBoxBehaviour<Data> : BoundingBoxBehaviour
{
    protected override void SetLocalTransform()
    {
        ChunkGridBehavior<Data> childBehaviour = childObject.GetComponent(typeof(ChunkGridBehavior<Data>)) as ChunkGridBehavior<Data>;
        IChunk<Data>[,,] chunks = childBehaviour.chunkGrid.Chunks;
        Vector3 nroChunks = new Vector3(chunks.GetLength(0), chunks.GetLength(1), chunks.GetLength(2));

        base.SetLocalTransform();
        Transform childTransform = childObject.transform;
        childTransform.localPosition = new Vector3(-0.5f, -0.5f, -0.5f);
        childTransform.localRotation = Quaternion.identity;
        childTransform.localScale = new Vector3(1 / nroChunks.x, 1 / nroChunks.y, 1 / nroChunks.z);
    }
}
