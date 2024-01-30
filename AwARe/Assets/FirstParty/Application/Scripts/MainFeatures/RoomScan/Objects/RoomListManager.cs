// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using System.Linq;
using AwARe.Data.Objects;
using UnityEngine;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomListManager : MonoBehaviour
    {
        private RoomListSerialization roomListSerialization;

        private RoomFileHandler fileHandler;
        [SerializeField] private ScreenshotManager screenshotManager;

        private void Awake()
        {
            fileHandler = new();
        }

        /// <summary>
        /// Saves the room by adding the Room to the local save file and the serialized rooms list if the room doesn't already exist.
        /// </summary>
        /// <param name="room">The room data to store locally.</param>
        /// <param name="anchors">The anchors for pairing virtual and real space.</param>
        /// <param name="screenshots">The screenshots for assisting anchor placement.</param>
        public void SaveRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            int index = RoomNameIndex(room.RoomName);

            if(index != -1) // The room already exists
                UpdateRoom(room, index, anchors, screenshots);
            else
                AddRoom(room, anchors, screenshots);
        }
        /// <summary>
        /// Deletes the room by removing the Room from the local save file and the serialized rooms list.
        /// </summary>
        /// <param name="roomIndex">The list-index of the serialized room to delete from roomListSerialization.</param>
        /// <param name="anchorCount">The amount of anchors (or screenshots) attached to this room.</param>
        public void DeleteRoom(int roomIndex, int anchorCount)
        {
            string roomName = GetSerRoomList().Rooms[roomIndex].RoomName;

            for (var i = 0; i < anchorCount; i++)
                screenshotManager.DeleteScreenshot(roomName, i);

            roomListSerialization.Rooms.RemoveAt(roomIndex);
            fileHandler.SaveRoomList("rooms", roomListSerialization);
        }

        /// <summary>
        /// Updates the room by modifying the Room in the local save file and the serialized rooms list.
        /// </summary>
        /// <param name="room">The room data to store locally.</param>
        /// <param name="roomIndex">The list-index of the serialized room to delete from roomListSerialization.</param>
        /// <param name="anchors">The anchors for pairing virtual and real space.</param>
        /// <param name="screenshots">The screenshots for assisting anchor placement.</param>
        public void UpdateRoom(Data.Logic.Room room, int roomIndex, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for (var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            roomListSerialization.Rooms[roomIndex] = new(room, anchors);
            fileHandler.SaveRoomList("rooms", roomListSerialization);
        }

        /// <summary>
        /// Adds a new room by saving the Room to the local save file and adding it to the serialized rooms list.
        /// </summary>
        /// <param name="room">The room data to store locally.</param>
        /// <param name="anchors">The anchors for pairing virtual and real space.</param>
        /// <param name="screenshots">The screenshots for assisting anchor placement.</param>
        public void AddRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for(var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            roomListSerialization.Rooms.Add(new(room, anchors));
            fileHandler.SaveRoomList("rooms", roomListSerialization);
        }

        /// <summary>
        /// Load in a list of rooms from roomListSerialization rooms.
        /// </summary>
        /// <param name="roomSer">The serialized room.</param>
        /// <param name="anchors">The anchors for pairing virtual and real space.</param>
        /// <returns>The room data.</returns>
        public Data.Logic.Room LoadRoom(RoomSerialization roomSer, List<Vector3> anchors)
        {
            return roomSer.ToRoom(anchors);
        }

        /// <summary>
        /// Checks if the list with serialized rooms has been loaded,
        /// and loads them if not.
        /// </summary>
        /// <returns>The serialized list of rooms.</returns>
        public RoomListSerialization GetSerRoomList()
        {
            roomListSerialization ??= LoadSerRoomList();
            return roomListSerialization;
        }

        /// <summary>
        /// Load in a serialized list of rooms.
        /// </summary>
        /// <returns>The serialized list of rooms.</returns>
        private RoomListSerialization LoadSerRoomList()
        {
            fileHandler ??= new();

            roomListSerialization = fileHandler.LoadRooms("rooms") ?? new();
            return roomListSerialization;
        }

        /// <summary>
        /// Finds the roomIndex of a room with the given name in the serialized room list.
        /// </summary>
        /// <param name="name">The name of the room.</param>
        /// <returns>The roomIndex of the room in the list or -1 if not found.</returns>
        private int RoomNameIndex(string name)
        {
            List<RoomSerialization> roomList = roomListSerialization.Rooms;
            for(var i = 0; i <  roomList.Count; i++)
                if (roomListSerialization.Rooms.Any(room => room.RoomName == name))
                    return i;
            return -1;
        }
    }
}