// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

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
        /// Returns a list of spawnpoints on which the objects are allowed to be spawned.
        /// </summary>
        /// <param name="room">The room in which the objects should be spawned.</param>
        /// <param name="path">The path in the room.</param>
        public List<Vector3> GetValidSpawnPoints(Room room, PathData path);
    }
}