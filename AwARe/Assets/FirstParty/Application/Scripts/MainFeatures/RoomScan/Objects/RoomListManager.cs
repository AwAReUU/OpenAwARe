// /*                                                                                       *\
//     This program has been developed by students from the bachelor Computer Science at
//     Utrecht University within the Software Project course.
//
//     (c) Copyright Utrecht University (Department of Information and Computing Sciences)
// \*                                                                                       */

using System.Collections.Generic;
using UnityEngine;

namespace AwARe.RoomScan.Objects
{
    /// <summary>
    /// Contains the Room and handles the different states within the Polygon scanning.
    /// </summary>
    public class RoomListManager : MonoBehaviour
    {
        private RoomListSerialization RoomListSerialization;

        private SaveLoadManager saveLoadManager;
        [SerializeField] private ScreenshotManager screenshotManager;

        private void Awake()
        {
            saveLoadManager = new();
        }

        public void SaveRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            int index = RoomNameIndex(room.RoomName);

            if(index != -1) // The room already exists
                UpdateRoom(room, index, anchors, screenshots);
            else
                AddRoom(room, anchors, screenshots);
        }

        public void DeleteRoom(int roomIndex, int anchorCount)
        {
            string roomName = GetSerRoomList().Rooms[roomIndex].RoomName;

            for (var i = 0; i < anchorCount; i++)
                screenshotManager.DeleteScreenshot(roomName, i);

            RoomListSerialization.Rooms.RemoveAt(roomIndex);
            saveLoadManager.SaveRoomList("rooms", RoomListSerialization);
        }

        public void UpdateRoom(Data.Logic.Room room, int index, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for (var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            RoomListSerialization.Rooms[index] = new(room, anchors);
            saveLoadManager.SaveRoomList("rooms", RoomListSerialization);
        }

        public void AddRoom(Data.Logic.Room room, List<Vector3> anchors, List<Texture2D> screenshots)
        {
            for(var i = 0; i < screenshots.Count; i++)
                screenshotManager.SaveScreenshot(screenshots[i], room, i);

            RoomListSerialization.Rooms.Add(new(room, anchors));
            saveLoadManager.SaveRoomList("rooms", RoomListSerialization);
        }

        /// <summary>
        /// Load in a list of rooms from roomListSerialization rooms.
        /// </summary>
        /// <param name="roomSer">The serialized room.</param>
        /// <param name="anchors">The set anchors.</param>
        /// <returns>The room data.</returns>
        public Data.Logic.Room LoadRoom(RoomSerialization roomSer, List<Vector3> anchors)
        {
            return roomSer.ToRoom(anchors);
        }

        public RoomListSerialization GetSerRoomList()
        {
            RoomListSerialization ??= LoadSerRoomList();
            return RoomListSerialization;
        }

        /// <summary>
        /// Load in a serialized list of rooms.
        /// </summary>
        /// <returns>The serialized list of rooms.</returns>
        private RoomListSerialization LoadSerRoomList()
        {
            saveLoadManager ??= new();

            RoomListSerialization = saveLoadManager.LoadRooms("rooms") ?? new();
            return RoomListSerialization;
        }

        private int RoomNameIndex(string name)
        {
            List<RoomSerialization> roomList = RoomListSerialization.Rooms;
            for(int i = 0; i <  roomList.Count; i++)
            foreach(RoomSerialization room in RoomListSerialization.Rooms)
                if(room.RoomName == name) return i;
            return -1;
        }
    }
}