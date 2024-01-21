using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using AwARe.AwARe.Data.Objects;
using AwARe.Data.Logic;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;
using AwARe.InterScenes.Logic;
using PlasticGui.Configuration.CloudEdition.Welcome;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

using Storage = AwARe.InterScenes.Objects.Storage;

namespace AwARe
{
    public class RoomListManager : MonoBehaviour
    {
        private RoomListOverviewScreen overviewRooms;
        
        public List<string> roomlist;
        public event Action OnRoomListChanged;
        public bool RoomListChangesMade { get; private set; }
        public string SelectedList { get; private set; } = null;
        public int IndexList { get; private set; } = -1;
        public List<string> Lists { get; private set; }

        
        [SerializeField] private TMP_InputField inputName;
        [SerializeField] private SaveLoadManager filehandler;
        

        private void Awake()
        {
            //roomlist = filehandler.LoadDataFromJson<List<string>>("rooms");
            //Lists = fileHandler.ReadFile();
            LoadLists();
            Debug.Log(roomlist);
            overviewRooms.DisplayRoomLists();


        }

        private void InitializeLists()
        {
           
        }

        /// <summary>
        /// Notifies objects subscribed to this action that the current IngredientList has been changed.
        /// </summary>
        private void NotifyRoomListChanged()
         {
             RoomListChangesMade = true;
             OnRoomListChanged?.Invoke();
         }


         /// <summary>
         /// Save all lists.
         /// </summary>
         public void SaveLists()
         {
             filehandler.SaveDataToJson("rooms",roomlist);

             RoomListChangesMade = false;
         }

         /// <summary>
         /// Save all lists.
         /// </summary>
         public void LoadLists()
         {
             SaveLoadManager saveLoadManager = GetComponent<SaveLoadManager>();

            Debug.Log("Loading lists. Directory Path: " + saveLoadManager.directoryPath);
            roomlist =saveLoadManager.LoadDataFromJson<List<string>>("rooms");
            RoomListChangesMade = false;
         }

         /// <summary>
         /// Adds a new, empty IngredientList to the overview and calls the fileHandler to save all lists.
         /// </summary>
         public void CreateList()
         {
             string inputname = inputName.text;
             Debug.Log("before"+roomlist);
             roomlist.Add(inputname);
             Debug.Log("after" +roomlist);
            NotifyRoomListChanged();
            SaveLists();
         }

         /// <summary>
         /// Removes the given list from the overview and calls the fileHandler to save all lists.
         /// </summary>
         /// <param name="list">The list to remove.</param>
         public void DeleteList(string save)
         {
             roomlist.Remove(save);
             NotifyRoomListChanged();
             SaveLists();
        }



    }
}


