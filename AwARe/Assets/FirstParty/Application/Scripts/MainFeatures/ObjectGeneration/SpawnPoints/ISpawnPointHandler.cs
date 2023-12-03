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
        public List<Vector3> GetValidSpawnPoints(Room room);
    }
}