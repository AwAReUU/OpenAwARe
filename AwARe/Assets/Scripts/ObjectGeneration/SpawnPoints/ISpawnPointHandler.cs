using System.Collections.Generic;
using UnityEngine;
using RoomScan;

namespace ObjectGeneration
{
    /// <summary>
    /// <c>ISpawnPointHandler</c> is an interface that mandates the implementation of GetValidSpawnPoints().
    /// </summary>
    public interface ISpawnPointHandler
    {
        public List<Vector3> GetValidSpawnPoints(Room room);
    }
}