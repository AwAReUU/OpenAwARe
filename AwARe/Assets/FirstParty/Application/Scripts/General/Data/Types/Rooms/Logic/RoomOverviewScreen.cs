// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using AwARe.RoomScan.Objects;
using UnityEngine;

namespace AwARe.Data.Logic
{
    /// <summary>
    /// <para>
    ///     Handles the UI of the RoomOverviewScreen screen.
    /// </para>
    /// <para>
    ///     Shows an overview of all <see cref="RoomListItem"/>s that have been created and saved by the user.
    /// </para>
    /// </summary>
    public class RoomOverviewScreen : MonoBehaviour
    {
        // The manager
        [SerializeField] private RoomManager manager;

        // UI elements to control
        [SerializeField] private GameObject listItemObject; //list item 'prefab'

        // Tracked UI elements
        readonly List<RoomListItem> list = new();

        private void OnEnable()
        {
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
        /// <param name="roomIndex">The index of the room to delete.</param>
        public void OnItemClick(int roomIndex)
        {
            manager.StartLoadingRoom(roomIndex);
        }

        /// <summary>
        /// Deletes the given room.
        /// </summary>
        /// <param name="roomIndex">The index of the room to delete.</param>
        public void OnDeleteButtonClick(int roomIndex)
        {
            manager.DeleteRoom(roomIndex);
        }
    }
}
