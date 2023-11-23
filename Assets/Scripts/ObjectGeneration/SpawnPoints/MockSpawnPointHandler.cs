using System.Collections.Generic;
using UnityEngine;

namespace ObjectGeneration
{
    /// <summary>
    /// Class <c>MockSpawnPointHandler</c> is an implementation of <see cref="ISpawnPointHandler"/>
    /// in which creates a hardcoded plane of spawnPoints. In this implementation, no room scan is necessary.
    /// This Implementation is only used for debugging/testing.
    /// </summary>
    public class MockSpawnPointHandler : ISpawnPointHandler
    {
        public MockSpawnPointHandler(float spacing = 0.1f) { gridSpacing = spacing; }

        /// <value>
        /// Minimal space in between spawnPoints
        /// </value>
        private readonly float gridSpacing;

        /// <summary>
        /// Just return a hardcoded "fake plane" of spawnpoints
        /// </summary>
        /// <returns>List of all spawnpoints</returns>
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
