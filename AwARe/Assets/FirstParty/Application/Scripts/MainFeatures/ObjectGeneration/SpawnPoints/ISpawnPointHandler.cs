using System.Collections.Generic;

using AwARe.RoomScan.Polygons.Logic;

using UnityEngine;

namespace AwARe.ObjectGeneration
{
    /// <summary>
    /// <c>ISpawnPointHandler</c> is an interface that mandates the implementation of GetValidSpawnPoints().
    /// </summary>
    public interface ISpawnPointHandler
    {
        /// <summary>
        /// returns a list of spawnpoints on which the objects are allowed to be spawned.
        /// </summary>
        public List<Vector3> GetValidSpawnPoints(Room room);
    }
}