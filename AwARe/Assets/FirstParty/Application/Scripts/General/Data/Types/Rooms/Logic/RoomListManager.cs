using System;
using System.Collections;
using System.Collections.Generic;

using AwARe.AwARe.Data.Objects;
using AwARe.Data.Logic;
using AwARe.Database.Logic;
using AwARe.IngredientList.Logic;
using AwARe.InterScenes.Logic;

using TMPro;

using UnityEngine;

using Storage = AwARe.InterScenes.Objects.Storage;

namespace AwARe
{
    public class RoomListManager : MonoBehaviour
    {
        public List<string> roomlist;
        public event Action OnRoomListChanged;
        public bool RoomListChangesMade { get; private set; }
        public string SelectedList { get; private set; } = null;
        public int IndexList { get; private set; } = -1;

        public SaveLoadManager filehandler;
        [SerializeField] private TMP_InputField inputName;

     

        private void Awake()
        {
            roomlist = filehandler.LoadDataFromJson<List<string>>("rooms");
            //Lists = fileHandler.ReadFile();
            InitializeLists();
        }

        private void InitializeLists()
         {
             
             if (roomlist.Count == 0)
             {
                 roomlist.Add("FirstSave");
                 NotifyRoomListChanged();
            }

            // Set default checked list.
            if (roomlist.Count > 0)
             {
                 IndexList = 0;
                 var list = roomlist[0];
                 //CheckedList = list;
                 SelectedList = list;
                 //Storage.Get().ActiveRoom = room;
             }
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
            roomlist = filehandler.LoadDataFromJson<List<string>>("rooms");
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
            filehandler.SaveDataToJson("rooms",roomlist);
         }

         /// <summary>
         /// Removes the given list from the overview and calls the fileHandler to save all lists.
         /// </summary>
         /// <param name="list">The list to remove.</param>
         public void DeleteList(string save)
         {
             roomlist.Remove(save);
             NotifyRoomListChanged();
            filehandler.SaveDataToJson("rooms",roomlist);
         }

         /// <summary>
         /// Changes the name of a list into it's newly give name.
         /// </summary>
         /// <param name="name"> The name that is to be given to the list. </param>
         /*public void ChangeListName(string name)
         {
             SelectedList.ChangeName(name);
             NotifyRoomListChanged();
         }*/


    }
}


