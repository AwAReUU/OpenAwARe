using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe
{
    /// <summary>
    /// Class <c>Vector3Serialization</c> is responsible for Serializing <see cref="Vector3"/> to <see cref="Vector3Serialization"/>.
    /// </summary>
    [System.Serializable]
    public class Vector3Serialization
    {
        /// <summary>
        /// Values for the x, y, z coordinates respectively.
        /// </summary>
        public float x, y, z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3Serialization"/> class,
        /// with a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="vector">Vector3 to use for initialization.</param>
        public Vector3Serialization(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        /// <summary>
        /// Converts the serialized Vector3 back to a Vector3 object.
        /// </summary>
        /// <returns>The deserialized Vector3.</returns>
        public Vector3 ToVector3() =>
            new(x, y, z);
    }

    /// <summary>
    /// Class <c>Vector3Serialization</c> is responsible for Serializing <see cref="Vector2"/> to <see cref="Vector2Serialization"/>.
    /// </summary>
    [System.Serializable]
    public class Vector2Serialization
    {
        /// <summary>
        /// Values for x, y coordinates respectively.
        /// </summary>
        public float x, y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Serialization"/> class,
        /// with a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="vector">Vector2 to use for initialization.</param>
        public Vector2Serialization(Vector2 vector)
        {
            x = vector.x;
            y = vector.y;
        }

        /// <summary>
        /// Converts the serialized Vector2 back to a Vector2 object.
        /// </summary>
        /// <returns>The deserialized Vector3.</returns>
        public Vector2 ToVector2() =>
            new(x, y);
    }
}
