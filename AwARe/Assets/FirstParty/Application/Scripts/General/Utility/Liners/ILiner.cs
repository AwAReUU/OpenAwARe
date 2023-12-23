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
    /// An interface for line constructors.
    /// </summary>
    public interface ILiner
    {
        /// <summary>
        /// Gets a line representing the GameObject.
        /// </summary>
        /// <value>A line representing the GameObject.</value>
        public Vector3[] Line { get; }
    }
}