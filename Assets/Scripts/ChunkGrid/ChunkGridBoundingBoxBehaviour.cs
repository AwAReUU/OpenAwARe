using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGridBoundingBoxBehaviour : BoundingBoxBehaviour
{
    protected override void SetLocalTransform()
    {
        IChunkGridSize chunkGridSize = childObject.GetComponent(typeof(IChunkGridSize)) as IChunkGridSize;
        (int gridSizeX, int gridSizeY, int gridSizeZ) = chunkGridSize.GridSize;
        Vector3 gridSize = new(gridSizeX, gridSizeY, gridSizeZ);
        
        base.SetLocalTransform();
        Transform childTransform = childObject.transform;
        childTransform.SetLocalPositionAndRotation(new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
        childTransform.localScale = new Vector3(1 / gridSize.x, 1 / gridSize.y, 1 / gridSize.z);
    }
}
