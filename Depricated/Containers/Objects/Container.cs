// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System;
using UnityEngine;

namespace AwARe.Objects
{
    /// <summary>
    /// A MonoBehaviour who grabs or instantiates a given GameObject and modifies the transform to fit in the container.
    /// </summary>
    public class Container : MonoBehaviour
    {
        /// <summary>
        /// The contained GameObject.
        /// </summary>
        protected GameObject childObject;
        /// <summary>
        /// The getter callback to obtain the contained GameObject.
        /// </summary>
        public Func<Transform, GameObject> getChild;

        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        protected void Start() 
        {
            childObject = getChild(this.transform);
            SetLocalTransform();
        }

        /// <summary>
        /// Corrects the local transform of the child class.
        /// </summary>
        protected virtual void SetLocalTransform() { }
    }
}