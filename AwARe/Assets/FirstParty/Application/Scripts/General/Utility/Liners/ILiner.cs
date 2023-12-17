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
    public interface ILiner
    {
        /// <summary>
        /// Compute a line representing the GameObject.
        /// </summary>
        /// <returns>A line representing the GameObject.</returns>
        public Vector3[] Line { get; }
    }
}