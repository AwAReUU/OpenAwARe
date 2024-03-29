// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Data.Logic;
using AwARe.RoomScan.Path;
using Ingredients = AwARe.IngredientList.Logic;

namespace AwARe.InterScenes
{
    /// <summary>
    /// An interface to the in between scenes stored data.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Gets or sets the currently active IngredientList.
        /// </summary>
        /// <value>
        /// The currently active IngredientList.
        /// </value>
        public Ingredients.IngredientList ActiveIngredientList { get; set; }

        
        /// <summary>
        /// Gets or sets the currently active Room.
        /// </summary>
        /// <value>
        /// The currently active Room.
        /// </value>
        public Room ActiveRoom { get; set; }

        /// <summary>
        /// Gets or sets the path belonging to the currently active Room.
        /// </summary>
        /// <value>
        /// The path belonging to the currently active Room.
        /// </value>
        public PathData ActivePath { get; set; }
    }
}