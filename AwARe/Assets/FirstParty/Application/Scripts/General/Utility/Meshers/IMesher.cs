// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// The mesh representing the Data; used to set the height.
    /// </summary>
    public interface IMesher
    {
        /// <summary>
        /// Compute a mesh representing the GameObject.
        /// </summary>
        /// <returns>A mesh representing the GameObject.</returns>
        public Mesh Mesh { get; }
    }
}