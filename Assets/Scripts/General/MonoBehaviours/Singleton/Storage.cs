using AwARe.DataStructures;
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
        private static Storage instance;

        private Data.Storage data;

        public Data.Storage Data
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

        public IngredientList ActiveIngredientList
        {
            get => Data.ActiveIngredientList;
            set => Data.ActiveIngredientList = value;
        }

        public static Storage Instantiate() =>
            new GameObject("Storage").AddComponent<Storage>();

        public static Storage Get() => 
            Singleton.Get(ref instance, Instantiate);
    }
}