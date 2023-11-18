using System.Collections.Generic;
using UnityEngine;

namespace ObjectGeneration
{
    public class MockSpawnPointHandler : ISpawnPointHandler
    {
        public MockSpawnPointHandler(float spacing = 0.1f) => gridSpacing = spacing;
        private readonly float gridSpacing;

        /// <summary>
        /// Just return a hardcoded "fake plane" of spawnpoints
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetValidSpawnPoints()
        {
            List<Vector3> result = new List<Vector3>();
            float y = -0.87f;

            for (float x = -8f; x <= -3; x += gridSpacing)
                for (float z = -8f; z <= -3; z += gridSpacing)
                    result.Add(new Vector3(x, y, z));

            return result;
        }
    }
}
