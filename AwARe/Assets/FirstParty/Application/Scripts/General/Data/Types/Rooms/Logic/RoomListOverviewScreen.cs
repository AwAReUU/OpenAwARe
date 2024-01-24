using System;

using AwARe.IngredientList.Objects;
using AwARe.InterScenes.Objects;
using PlasticGui.Configuration.CloudEdition.Welcome;
using System.Collections;
using System.Collections.Generic;

using AwARe.AwARe.Data.Objects;

using NSubstitute.ReturnsExtensions;

using TMPro;

using UnityEngine;
using UnityEngine.UI;
using AwARe.IngredientList.Logic;

using AwARe.Data.Logic;
using AwARe.Objects;
using AwARe.RoomScan.Objects;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using AwARe.RoomScan.Path.Objects;

using UnityEngine.Analytics;

using Object = System.Object;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the ingredient list overview screen.
    /// </para>
    /// <para>
    ///     Shows an overview of all <see cref="IngredientList"/>s that have been created and saved by the user.
    ///     Allows the user to select a list to edit and tick a checkbox of the list they want to be visualized in AR.
    /// </para>
    /// </summary>
    public class RoomListOverviewScreen : MonoBehaviour
    {
        // The parent element
        // The manager
        [SerializeField] private RoomManager manager;
        [SerializeField] private PathManager pathManager;


        // UI elements to control/copy
        [SerializeField] private GameObject listItemObject; //list item 'prefab'
        [SerializeField] private GameObject nameSaveRoom;
        public SaveLoadManager filehandler;
        public List<Room> roomList { get; set; }

        //public Data.Objects.Room room2;
        // Tracked UI elements
        readonly List<GameObject> lists = new();

        private void OnEnable() { DisplayRoomLists(roomList); }

        private void OnDisable() { RemoveLists(); }

        /// <summary>
        /// Creates GameObjects with buttons to select or destroy every ingredient list.
        /// </summary>
        /// 
      
        private void Awake()
        {
            //roomlist = filehandler.LoadDataFromJson<List<string>>("rooms");
            //Lists = fileHandler.ReadFile();
            roomList = manager.LoadRoomList();
            //DisplayRoomLists(roomList);
            
        }

        public void DisplayRoomLists(List<Room> roomList)
        {
            
                RemoveLists();
                //roomList = manager.LoadRoomList();
                // Now you can work with the list of Room objects
                foreach (Room room in roomList)
                {
                // create a new list item to display this list
                     GameObject itemObject = Instantiate(listItemObject, listItemObject.transform.parent);
                    // Set the ingredients of the item and keep track of it.
                    TMP_Text buttontext = itemObject.GetComponentInChildren<TMP_Text>();
                     buttontext.text = room.RoomName;
                    itemObject.SetActive(true);
                    // Set the ingredients of the item and keep track of it.
                    lists.Add(itemObject);
                    // Check if the room name already exists
                   
            }
            
        }
        /// <summary>
        /// Destroys all currently displayed GameObjects in the ScrollView.
        /// </summary>
        private void RemoveLists()
        {
            foreach (GameObject o in lists )
                Destroy(o.gameObject);
            lists.Clear();
        }


        /// <summary>
        /// Calls an instance of manager to create a new ingredient list, then displays the new list of ingredient lists.
        /// </summary>
        public void OnAddListButtonClick(Room currentroom)
        {
            //manager.CreateList(currentroom);
            //DisplayRoomLists(roomList);
        }

        
        public void OnConfirmNameButton()
        {
            nameSaveRoom.SetActive(false);

            if (roomList.Where(obj => obj.RoomName == manager.inputName.text).Count() > 0)
            {
                Debug.Log(" this already exist");
            }
            else manager.SaveClick();
            //DisplayRoomLists(roomList); 
        }


        public void DeleteList(string name)
        {
            roomList = manager.LoadRoomList();
            Debug.Log(roomList.Count);
            roomList.Remove(roomList.Where(obj => obj.RoomName == name).SingleOrDefault());
            Debug.Log(roomList.Count);
            manager.UpdateRoomList(roomList);
            DisplayRoomLists(roomList);
        }

        public void OnRoomItemClick(string name)
        {
            Data.Logic.Room room;
            room = manager.ChooseRoom(name);
            manager.MakeRoom(room);
        }

        public void OnSaveNewRoomClick()
        {
            nameSaveRoom.SetActive(true);
            DisplayRoomLists(roomList);
        }

        /// <summary>
        /// Loads the Home scene.
        /// </summary>
        public void OnBackButtonClick() { SceneSwitcher.Get().LoadScene("Home"); }


    }

}
