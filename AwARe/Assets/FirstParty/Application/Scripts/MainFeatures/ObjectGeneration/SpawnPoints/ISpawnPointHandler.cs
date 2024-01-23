using System.Collections.Generic;
using AwARe.Data.Logic;
using AwARe.RoomScan.Path;
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
        public List<Vector3> GetValidSpawnPoints(Room room, PathData path);
    }
}