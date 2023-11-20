using IngredientLists;
using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using Data = AwARe.DataStructures;

namespace AwARe.MonoBehaviours
{
    public class Storage : MonoBehaviour, Data.IStorage
    {
        private static Data.Storage data;

        public static Data.Storage Data
        {
            get => data ?? LoadStorage();
            set => data = value;
        }

        public static Storage Get() { 
            var storage = GameObject.FindObjectOfType<Storage>();

            if (storage == null)
                storage = Instantiate(Resources.Load<Storage>("Prefabs/Storage/Storage"));

            return storage;
        }

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.
            LoadStorage();
            
            DontDestroyOnLoad(gameObject);
        }

        private static Data.Storage LoadStorage()
        {
            data = new Data.Storage();
            return data;
        }

        public IngredientList ActiveIngredientList 
        { 
            get => Data.ActiveIngredientList;
            set => Data.ActiveIngredientList = value;
        }
    }
}