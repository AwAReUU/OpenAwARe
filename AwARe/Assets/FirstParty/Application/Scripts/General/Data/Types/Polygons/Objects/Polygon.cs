// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using UnityEngine;

namespace AwARe.Data.Objects
{
    /// <summary>
    /// The component for storing polygon data.
    /// </summary>
    public class Polygon : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets and sets the data-type Polygon represented by this GameObject.
        /// </summary>
        /// <value>
        /// The data-type Polygon represented.
        /// </value>
        public Logic.Polygon Data { get; set; }
    }
}