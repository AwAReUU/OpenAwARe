using UnityEngine;

using AwARe.Objects;

namespace AwARe.Data.Objects
{
    public class ChunkGridContainer : Container
    {
        /// <inheritdoc/>
        protected override void SetLocalTransform()
        {
            IChunkGridSize chunkGridSize = childObject.GetComponent<IChunkGridSize>();
            Vector3 gridSize = chunkGridSize.GridSize.ToVector3();

            Transform childTransform = childObject.transform;
            childTransform.SetLocalPositionAndRotation(new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity);
            childTransform.localScale = new Vector3(1 / gridSize.x, 1 / gridSize.y, 1 / gridSize.z);
        }
    }
}

