using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGridBoundingBoxBehaviour : BoundingBoxBehaviour
{
    protected override void SetLocalTransform()
    {
        IChunkGridSize chunkGridSize = childObject.GetComponent(typeof(IChunkGridSize)) as IChunkGridSize;
        (int nroChunksX, int nroChunksY, int nroChunksZ) = chunkGridSize.NroChunks;
        Vector3 nroChunks = new(nroChunksX, nroChunksY, nroChunksZ);
        
        base.SetLocalTransform();
        Transform childTransform = childObject.transform;
        childTransform.SetLocalPositionAndRotation(new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
        childTransform.localScale = new Vector3(1 / nroChunks.x, 1 / nroChunks.y, 1 / nroChunks.z);
    }
}
