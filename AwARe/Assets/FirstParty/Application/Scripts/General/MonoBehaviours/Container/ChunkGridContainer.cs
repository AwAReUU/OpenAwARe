using AwARe.DataStructures;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Data = AwARe.DataStructures;

namespace AwARe.MonoBehaviours
{
    public class ChunkGridContainer : Container
    {
        protected override void SetLocalTransform()
        {
            Data.IChunkGridSize chunkGridSize = childObject.GetComponent<Data.IChunkGridSize>();
            Vector3 gridSize = chunkGridSize.GridSize.ToVector3();

            Transform childTransform = childObject.transform;
            childTransform.SetLocalPositionAndRotation(new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
            childTransform.localScale = new Vector3(1 / gridSize.x, 1 / gridSize.y, 1 / gridSize.z);
        }
    }
}

