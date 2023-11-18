using System.Collections.Generic;
using UnityEngine;

namespace ObjectGeneration
{
    public interface ISpawnPointHandler
    {
        public List<Vector3> GetValidSpawnPoints();
    }
}