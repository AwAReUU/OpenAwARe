// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using AwARe.Logic;
using UnityEngine;
using Ingredients = AwARe.IngredientList.Logic;
using Rooms = AwARe.RoomScan.Polygons.Logic;

namespace AwARe.InterScenes.Objects
{
    /// <summary>
    /// The Singleton containing the in-between-scenes stored data.
    /// </summary>
    public class Storage : MonoBehaviour, IStorage
    {
        // Singleton instance
        private static Storage instance;

        private Logic.Storage data;

        /// <summary>
        /// Gets or sets the stored data. Load empty data if needed.
        /// </summary>
        /// <value>The currently stored data.</value>
        public Logic.Storage Data
        {
            get => data ??= new();
            set => data = value;
        }
        
        /// <summary>
        /// Called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            Singleton.Awake(ref instance, this);
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// Called when the behaviour component is destroyed.
        /// </summary>
        protected virtual void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        /// <inheritdoc/>
        public Ingredients.IngredientList ActiveIngredientList
        {
            get => Data.ActiveIngredientList;
            set => Data.ActiveIngredientList = value;
        }

        /// <inheritdoc/>
        public Rooms.Room ActiveRoom
        {
            get => Data.ActiveRoom;
            set => Data.ActiveRoom = value;
        }
        
        /// <summary>
        /// Instantiate a new instance of itself.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static Storage Instantiate() =>
            new GameObject("Storage").AddComponent<Storage>();
        
        /// <summary>
        /// Get its current instance.
        /// Instantiate a new instance if necessary.
        /// </summary>
        /// <returns>An instance of itself.</returns>
        public static Storage Get() => 
            Singleton.Get(ref instance, Instantiate);
    }
}