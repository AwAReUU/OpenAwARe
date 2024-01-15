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
    /// An interface for mesh constructors.
    /// </summary>
    public interface IMesherLogic
    {
        /// <summary>
        /// Gets a mesh representing the GameObject.
        /// </summary>
        /// <value>A mesh representing the GameObject.</value>
        public Mesh Mesh { get; }
    }
}