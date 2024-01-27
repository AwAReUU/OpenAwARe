// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.RoomScan.Objects;
using TMPro;
using UnityEngine;

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
    public class RoomOverviewScreen : MonoBehaviour
    {
        // The manager
        [SerializeField] private RoomManager manager;

        // UI elements to control
        [SerializeField] private GameObject listItemObject; //list item 'prefab'
        [SerializeField] private GameObject nameInputWindow;
        [SerializeField] private GameObject saveButton;
        [SerializeField] public TMP_InputField nameInput;

        // Tracked UI elements
        readonly List<RoomListItem> list = new();

        private void OnEnable()
        {
            nameInputWindow.SetActive(false);
            DisplayList();
        }

        private void OnDisable()
        { 
            RemoveList(); 
        }

        /// <summary>
        /// For all the rooms in the rooms file make a button with the rooms name on it.
        /// </summary>
        public void DisplayList()
        {
            RemoveList();

            List<RoomSerialization> roomList = manager.RoomListSerialization.Rooms;

            if(roomList.Count == 0)
                Debug.Log("Empty room list");

            for(int i = 0; i < roomList.Count; i++)
            {
                // create a new list item to display this list
                GameObject itemObject = Instantiate(listItemObject, listItemObject.transform.parent);
                itemObject.SetActive(true);

                // Set the room of the item.
                RoomListItem item = itemObject.GetComponent<RoomListItem>();
                int index = i;
                item.SetItem(i, roomList[i].RoomName);
                list.Add(item);
            }
        }

        /// <summary>
        /// Destroys all currently displayed GameObjects in the ScrollView.
        /// </summary>
        private void RemoveList()
        {
            foreach (RoomListItem o in list)
                Destroy(o.gameObject);
            list.Clear();
        }

        /// <summary>
        /// Calls an instance of manager to start loading the room that has been clicked.
        /// </summary>
        public void OnItemClick(int roomIndex)
        {
            manager.StartLoadingRoom(roomIndex);
        }

        /// <summary>
        /// Deletes the given room.
        /// </summary>
        /// <param name="room">The room to delete.</param>
        public void OnDeleteButtonClick(int roomIndex)
        {
            manager.DeleteRoom(roomIndex);
        }

        /// <summary>
        /// Checks if room name already exists in the rooms file;
        /// if not then it will be saved and will show up in the list of roomsaves.
        /// </summary>
        public void OnConfirmNameButton()
        {
            if (manager.RoomListSerialization.Rooms.Where(obj => obj.RoomName == nameInput.text).Count() > 0)
                Debug.LogError("This name already exists");
            else
            {
                manager.SaveRoom();
                nameInputWindow.SetActive(false);
            }
        }

        /// <summary>
        /// Loads the informationscreen to put a name in for the roomlist item.
        /// </summary>
        public void OnSaveNewRoomClick()
        {
            nameInputWindow.SetActive(true);
            saveButton.SetActive(false);

            DisplayList();
        }
    }
}
