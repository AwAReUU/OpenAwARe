using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AwARe
{
    using System;
    using System.Collections.Generic;

    namespace AwARe.Data.Objects
    {
        /// <summary>
        /// Data structure that stores a list name and a Dictionary of <see cref="Ingredient"/>s with their quantity and <see cref="QuantityType"/>.
        /// </summary>
        public class RoomSave
        {

            /// <summary>
            /// Gets the name given to this IngredientList.
            /// </summary>
            /// <value>The name currently given to this list.</value>
            public string RoomName { get; private set; }

            public void ChangeName(string name) { RoomName = name; }

        }
    }
}
