

using Rooms = AwARe.RoomScan.Polygons.Logic;
using Ingredients = AwARe.IngredientList.Logic;
using AwARe.Logic;

using UnityEngine;

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