

using Rooms = AwARe.RoomScan.Polygons.Logic;
using Ingredients = AwARe.IngredientList.Logic;
using AwARe.Logic;

using UnityEngine;
using System.Collections.Generic;

namespace AwARe.InterScenes.Objects
{
    public class Storage : MonoBehaviour, IStorage
    {
        private static Storage instance;

        private Logic.Storage data;

        public Logic.Storage Data
        {
            get => data ??= new();
            set => data = value;
        }
        
        // Save and load anchor ID
        public string SavedAnchorId
        {
            get => Data.SavedAnchorId;
            set => Data.SavedAnchorId = value;
        }

        public string SavedPolygonKey
        {
            get => Data.SavedPolygonKey;
            set => Data.SavedPolygonKey = value;
        }

        // Save and load polygon as JSON
        // Save and load polygon JSON
        // Save and load polygon JSON using a Dictionary with integer keys
        public Dictionary<int, string> SavedPolygons
        {
            get => Data.SavedPolygons ??= new Dictionary<int, string>();
            set => Data.SavedPolygons = value;
        }

        private void Awake()
        {
            Singleton.Awake(ref instance, this);
            DontDestroyOnLoad(this.gameObject);
        }

        protected virtual void OnDestroy() =>
            Singleton.OnDestroy(ref instance, this);

        public Ingredients.IngredientList ActiveIngredientList
        {
            get => Data.ActiveIngredientList;
            set => Data.ActiveIngredientList = value;
        }
        
        public Rooms.Room ActiveRoom
        {
            get => Data.ActiveRoom;
            set => Data.ActiveRoom = value;
        }

        public static Storage Instantiate() =>
            new GameObject("Storage").AddComponent<Storage>();

        public static Storage Get() => 
            Singleton.Get(ref instance, Instantiate);
    }
}