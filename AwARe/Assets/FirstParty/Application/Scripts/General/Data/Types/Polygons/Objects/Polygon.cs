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
    /// The mesh representing the Data; used to set the height.
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